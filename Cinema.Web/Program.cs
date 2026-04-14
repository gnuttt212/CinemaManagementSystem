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

builder.Services.AddScoped<ICinemaAdoNetDAL, CinemaAdoNetDAL>();
builder.Services.AddScoped<IDichVuBUS, DichVuBUS>();
builder.Services.AddScoped<IPhimBUS, PhimBUS>();
builder.Services.AddScoped<IHoaDonBUS, HoaDonBUS>();
builder.Services.AddScoped<INguoiDungBUS, NguoiDungBUS>();

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