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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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