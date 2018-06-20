using System;
using AspergillosisEPR.Data.DatabaseSeed;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FluentScheduler;
using AspergillosisEPR.Data;

namespace AspergillosisEPR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AspergillosisContext>();                  
                    var context2 = services.GetRequiredService<ApplicationDbContext>();
                    AspergillosisDatabaseSeeder.SeedDatabase(context);
                    AppDbInitializer.Initialize(context2);                   
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
           
            host.Run();

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseUrls("http://0*:5000/")
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

    }
}
