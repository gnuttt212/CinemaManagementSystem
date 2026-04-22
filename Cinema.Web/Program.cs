using Microsoft.EntityFrameworkCore;
using Cinema.DAL.Models;
using Cinema.DAL.AdoNet;
using Cinema.BUS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<QuanLyRapPhimContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// DAL
builder.Services.AddScoped<ICinemaAdoNetDAL, CinemaAdoNetDAL>();

// BUS
builder.Services.AddScoped<IPhimBUS, PhimBUS>();
builder.Services.AddScoped<IHoaDonBUS, HoaDonBUS>();
builder.Services.AddScoped<IKhachHangBUS, KhachHangBUS>();
builder.Services.AddScoped<INhanVienBUS, NhanVienBUS>();
builder.Services.AddScoped<IDoAnBUS, DoAnBUS>();
builder.Services.AddScoped<IKhuyenMaiBUS, KhuyenMaiBUS>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();