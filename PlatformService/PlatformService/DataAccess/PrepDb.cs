using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;
using System.Linq;

namespace PlatformService.DataAccess
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app,bool isProduction)
        {
            using var ss = app.ApplicationServices.CreateScope();
            SeedData(ss.ServiceProvider.GetService<AppDbContext>(), isProduction);
        }

        private static void SeedData(AppDbContext context, bool isProduction) 
        {
            if (true)
            {
                System.Console.WriteLine("apply migration");
                try
                {
                    context.Database.Migrate();
                }
                catch (System.Exception ex)
                {

                    System.Console.WriteLine(ex.Message);
                }
            }
            if (!context.Platforms.Any())
            {
                context.Platforms.AddRange(
                    new Platform() {Name="DotNet",Publisher="Microsoft",Cost="Free" },
                    new Platform() { Name = "Sql Server", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "Kubernetes", Publisher = "CNCF", Cost = "Free" }
                    );
                context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("have data");
            }
        }
    }
}
