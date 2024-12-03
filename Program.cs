using GoldenV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace GoldenV
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

            builder.Services.AddDbContext<GoldenVglampingContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BloggingDatabase")));

            /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
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
