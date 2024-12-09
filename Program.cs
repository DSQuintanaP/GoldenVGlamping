using GoldenV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

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

            void ConfigureServices(IServiceCollection services)
            {
                var mapperConfig = new MapperConfiguration(gold =>
                {
                    gold.AddProfile(new MappingProfile());
                });
                IMapper mapper = mapperConfig.CreateMapper();
                services.AddSingleton(mapper);
                services.AddMvc();
            }

            builder.Services.AddControllers(options =>
            {
                options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
            });

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
