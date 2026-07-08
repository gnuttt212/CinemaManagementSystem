using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Cinema.DAL.Models;
using Cinema.DAL.AdoNet;
using Cinema.BUS;
using Serilog;
using System.Text.Json;

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
        .WriteTo.Console(new Serilog.Formatting.Compact.CompactJsonFormatter())
        .WriteTo.File(
            new Serilog.Formatting.Compact.CompactJsonFormatter(),
            path: Path.Combine("logs", "cinema-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30));

    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddMemoryCache();
    builder.Services.AddSignalR();
    builder.Services.AddResponseCompression();

    builder.Services.AddDbContext<QuanLyRapPhimContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

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
    // Health Checks — liveness + SQL Server readiness
    // -----------------------------------------------------------------------
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
            name: "sqlserver",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "db" });

    // -----------------------------------------------------------------------
    // Forwarded Headers — required when running behind a reverse proxy (Nginx)
    // so the app sees the correct client IP, scheme (HTTPS), and host.
    // -----------------------------------------------------------------------
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Trust all proxies in Docker network (clear defaults to accept any)
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    builder.Services.AddScoped<ICinemaAdoNetDAL, CinemaAdoNetDAL>();
    builder.Services.AddScoped<IPhimBUS, PhimBUS>();
    builder.Services.AddScoped<IHoaDonBUS, HoaDonBUS>();
    builder.Services.AddScoped<IKhachHangBUS, KhachHangBUS>();
    builder.Services.AddScoped<INhanVienBUS, NhanVienBUS>();
    builder.Services.AddScoped<IDoAnBUS, DoAnBUS>();
    builder.Services.AddScoped<IKhuyenMaiBUS, KhuyenMaiBUS>();
    builder.Services.AddScoped<IPhongChieuBUS, PhongChieuBUS>();

    var app = builder.Build();

    // -----------------------------------------------------------------------
    // Middleware pipeline
    // -----------------------------------------------------------------------

    // ForwardedHeaders MUST be first so all subsequent middleware sees correct
    // client IP and scheme.
    app.UseForwardedHeaders();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // Only redirect to HTTPS in development (when not behind a reverse proxy).
    // In production, Nginx handles HTTPS termination.
    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseResponseCompression();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    // Serilog request logging — logs method, path, status code, elapsed time.
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

    // Liveness probe: is the process alive and able to handle requests?
    app.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => false, // No dependency checks — just "is the app running?"
        ResponseWriter = WriteHealthCheckResponse
    });

    // Readiness probe: is the app ready to serve traffic? (includes DB check)
    app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = WriteHealthCheckResponse
    });

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