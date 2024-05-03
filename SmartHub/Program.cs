using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartHub.DbIdentity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using SmartHub;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SmartHub.DataContext;
using System.Net.WebSockets;
using Microsoft.AspNetCore.WebSockets;

namespace SmartHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<WebSocketManager>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDataContext>(options =>
            {
                options.UseSqlite("Data Source=identity.db");
            });

            builder.Services.AddDbContext<DataDbContext>(options =>
            {
                options.UseSqlite("Data Source=dataContext.db");
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = false;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDataContext>().AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "SmartHub";
                options.Cookie.HttpOnly = false;
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });

            builder.Services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            var app = builder.Build();

            app.UseWebSockets();

            app.UseMiddleware<DeviceMiddleware>();

            // Применение миграций
            using (var serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDataContext>();
                dbContext.Database.Migrate();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // Значение по умолчанию для HSTS составляет 30 дней. Вы можете изменить его для сценариев продакшена, см. https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}