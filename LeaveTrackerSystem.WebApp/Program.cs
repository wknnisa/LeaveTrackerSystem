using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Infrastructure.Persistence;
using LeaveTrackerSystem.Infrastructure.Repositories;
using LeaveTrackerSystem.Infrastructure.Services;
using LeaveTrackerSystem.WebApp.Logging;
using LeaveTrackerSystem.WebApp.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

//LoggingExtensions.ConfigureSerilog();

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog();

// Database
//builder.Services.AddDbContext<LeaveTrackerDbContext>(options => 
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// SQLite database path (Azure-safe)
var dbPath = Path.Combine(@"C:\home\site", "leavetracker.db");

builder.Services.AddDbContext<LeaveTrackerDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// MVC
builder.Services.AddControllersWithViews();

// Register repositories
builder.Services.AddScoped<ILeaveRequestRepository, EfLeaveRequestRepository>();
builder.Services.AddScoped<ILeaveTypeRepository, EfLeaveTypeRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add session services with configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LeaveTrackerDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
