using Microsoft.EntityFrameworkCore;
using MassTransit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.DataProtection;
using Cinema.Web.Modules.Identity.Entities;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.Web.Modules.Booking.Entities;
using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Catalog.Data;
using Cinema.Web.Modules.Booking.Data;
using Cinema.Web.Modules.Identity.Services;
using Cinema.Web.Modules.Catalog.Services;
using Cinema.Web.Modules.Booking.Services;
using Cinema.Web.Services;
using Serilog;
using StackExchange.Redis;
using Minio;
using Prometheus;
using System.Text.Json;
using Cinema.Web.BackgroundServices;

// ---------------------------------------------------------------------------
// Bootstrap Serilog early so unhandled startup errors are captured.
// ---------------------------------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // -----------------------------------------------------------------------
    // Serilog — replaces the default Microsoft logger
    // -----------------------------------------------------------------------
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "CinemaManagementSystem")
        .WriteTo.Console()
        .WriteTo.File(
            new Serilog.Formatting.Compact.CompactJsonFormatter(),
            path: Path.Combine("logs", "cinema-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30));

    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddResponseCompression();

    // -----------------------------------------------------------------------
    // Redis — distributed cache, session store, data protection keys
    // -----------------------------------------------------------------------
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

    var redisConnection = ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints = { redisConnectionString },
        AbortOnConnectFail = false, // Don't crash if Redis is temporarily unavailable
        ConnectRetry = 5,
        ConnectTimeout = 5000,
    });
    builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(redisConnection);
        options.InstanceName = "cinema:cache:";
    });

    builder.Services.AddDataProtection()
        .SetApplicationName("CinemaManagementSystem")
        .PersistKeysToStackExchangeRedis(redisConnection, "cinema:dataprotection:keys");

    // -----------------------------------------------------------------------
    // SignalR — with Redis backplane for horizontal scaling
    // -----------------------------------------------------------------------
    builder.Services.AddSignalR()
        .AddStackExchangeRedis(redisConnectionString, options =>
        {
            options.Configuration.ChannelPrefix = RedisChannel.Literal("cinema:signalr:");
        });

    // -----------------------------------------------------------------------
    // Database
    // -----------------------------------------------------------------------
    builder.Services.AddDbContext<IdentityDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddDbContext<CatalogDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddDbContext<BookingDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // -----------------------------------------------------------------------
    // Session — backed by Redis distributed cache
    // -----------------------------------------------------------------------
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // -----------------------------------------------------------------------
    // Authentication
    // -----------------------------------------------------------------------
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "PLACEHOLDER_CLIENT_ID";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "PLACEHOLDER_CLIENT_SECRET";
        options.SaveTokens = true;
    });

    // -----------------------------------------------------------------------
    // Health Checks — SQL Server + Redis
    // -----------------------------------------------------------------------
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
            name: "sqlserver",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "db" })
        .AddRedis(
            redisConnectionString,
            name: "redis",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "cache" });

    // -----------------------------------------------------------------------
    // Forwarded Headers — required behind reverse proxy (Nginx)
    // -----------------------------------------------------------------------
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    // -----------------------------------------------------------------------
    // Poster Storage — MinIO (production) or Local filesystem (development)
    // -----------------------------------------------------------------------
    var minioEndpoint = builder.Configuration["MinIO:Endpoint"];
    if (!string.IsNullOrEmpty(minioEndpoint))
    {
        builder.Services.AddMinio(configureClient => configureClient
            .WithEndpoint(minioEndpoint)
            .WithCredentials(
                builder.Configuration["MinIO:AccessKey"] ?? "minioadmin",
                builder.Configuration["MinIO:SecretKey"] ?? "minioadmin")
            .WithSSL(builder.Configuration.GetValue<bool>("MinIO:UseSSL")));

        builder.Services.AddSingleton<IPosterStorageService, MinioPosterStorageService>();
        Log.Information("Poster storage: MinIO ({Endpoint})", minioEndpoint);
    }
    else
    {
        builder.Services.AddSingleton<IPosterStorageService, LocalPosterStorageService>();
        Log.Information("Poster storage: Local filesystem");
    }

    // -----------------------------------------------------------------------
    // Business Services
    // -----------------------------------------------------------------------
    builder.Services.AddSingleton<MongoDbContext>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<ICinemaAdoNetDAL, CinemaAdoNetDAL>();
    builder.Services.AddScoped<IPhimBUS, PhimBUS>();
    builder.Services.AddScoped<IHoaDonBUS, HoaDonBUS>();
    builder.Services.AddScoped<IKhachHangBUS, KhachHangBUS>();
    builder.Services.AddScoped<INhanVienBUS, NhanVienBUS>();
    builder.Services.AddScoped<IDoAnBUS, DoAnBUS>();
    builder.Services.AddScoped<IKhuyenMaiBUS, KhuyenMaiBUS>();
    builder.Services.AddScoped<IPhongChieuBUS, PhongChieuBUS>();

    // -----------------------------------------------------------------------
    // RabbitMQ Services & MassTransit
    // -----------------------------------------------------------------------
    builder.Services.AddMassTransit(x =>
    {
        x.AddEntityFrameworkOutbox<BookingDbContext>(o =>
        {
            o.UseSqlServer();
            o.UseBusOutbox();
        });

        x.UsingRabbitMq((context, cfg) =>
        {
            var rmqConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/";
            cfg.Host(rmqConnectionString);
            cfg.ConfigureEndpoints(context);
        });
    });

    var app = builder.Build();

    // -----------------------------------------------------------------------
    // Middleware pipeline
    // -----------------------------------------------------------------------

    app.UseForwardedHeaders();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseResponseCompression();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    // Prometheus HTTP metrics
    app.UseHttpMetrics();

    // Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        };
    });

    app.UseAuthentication();
    app.UseAuthorization();

    // -----------------------------------------------------------------------
    // Health Check Endpoints
    // -----------------------------------------------------------------------

    app.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => false,
        ResponseWriter = WriteHealthCheckResponse
    });

    app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = WriteHealthCheckResponse
    });

    // Prometheus metrics endpoint
    app.MapMetrics();

    app.MapHub<Cinema.Web.Hubs.SeatHub>("/seatHub");

    app.MapControllerRoute(
        name: "MyAreas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// ---------------------------------------------------------------------------
// Health check JSON response writer
// ---------------------------------------------------------------------------
static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var result = new
    {
        status = report.Status.ToString(),
        duration = report.TotalDuration.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            duration = e.Value.Duration.ToString(),
            description = e.Value.Description,
            exception = e.Value.Exception?.Message
        })
    };

    return context.Response.WriteAsync(
        JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
}
