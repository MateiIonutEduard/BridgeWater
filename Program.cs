using BridgeWater.Data;
using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BridgeWater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add PostgreSQL database linker to entities.
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BridgeContext>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresBW")));

            // now, need MongoDB database config 
            builder.Services.Configure<BridgeWaterSettings>(builder.Configuration.GetSection("BridgeWaterDB"));

            // get appSettings section
            builder.Services.Configure<AppSettings>(
                    builder.Configuration.GetSection(nameof(AppSettings)));

            // register AppSettings as singleton service
            builder.Services.AddSingleton<IAppSettings>(sp =>
                sp.GetRequiredService<IOptions<AppSettings>>().Value);

            // admin settings config section
            builder.Services.Configure<AdminSettings>(
                    builder.Configuration.GetSection(nameof(AdminSettings)));

            // register admin settings as singleton service
            builder.Services.AddSingleton<IAdminSettings>(sp =>
                sp.GetRequiredService<IOptions<AdminSettings>>().Value);

            // now, it is time, to declare admin service, as singleton
            builder.Services.AddSingleton<IAdminService, AdminService>();

            // add crypto service, needed to password encryption
            builder.Services.AddSingleton<ICryptoService, CryptoService>();

            // inject product service
            builder.Services.AddTransient<IProductService, ProductService>();

            // add plant service
            builder.Services.AddSingleton<PlantService>();

            // Add cookie authentication
            builder.Services.AddAuthentication("CookieAuthentication")
                .AddCookie("CookieAuthentication", config =>
                {
                    config.Cookie.Name = "LoginCookie";
                    config.LoginPath = "/Account";
                });

            builder.Services.AddControllersWithViews();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}