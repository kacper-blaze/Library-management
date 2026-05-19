using lab10.Configuration;
using lab10.Models;
using lab10.Services;
using Microsoft.EntityFrameworkCore;

namespace lab10;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        
        // 1. ADD THIS: Register session services and configure options
        builder.Services.AddDistributedMemoryCache(); // Required for session storage
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
            options.Cookie.HttpOnly = true;                        
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(AppSettings.GetConfiguration().ConnectionString));

        var app = builder.Build();

        // Seed database on startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DbSeeder.SeedAsync(dbContext);
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // 2. ADD THIS: Enable session middleware 
        // CRITICAL: This MUST go after UseRouting() and BEFORE UseAuthorization()
        app.UseSession(); 

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}