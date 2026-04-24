using Microsoft.EntityFrameworkCore;
using Cinema.DAL.Models;
using Cinema.DAL.AdoNet;
using Cinema.BUS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

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
    // Cần cung cấp ClientId và ClientSecret trong appsettings.json thực tế
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "PLACEHOLDER_CLIENT_ID";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "PLACEHOLDER_CLIENT_SECRET";
    options.SaveTokens = true;
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();