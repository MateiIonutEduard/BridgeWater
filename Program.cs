using BridgeWater.Data;
using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.EntityFrameworkCore;

namespace BridgeWater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BridgeContext>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresBW")));

            builder.Services.Configure<BridgeWaterSettings>(builder.Configuration.GetSection("BridgeWaterDB"));
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddSingleton<PlantService>();
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}