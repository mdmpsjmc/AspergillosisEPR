using System;
using AspergillosisEPR.Data.DatabaseSeed;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FluentScheduler;
using AspergillosisEPR.Data;
using AspergillosisEPR.BackgroundTasks.Logging;
using NLog.Web;

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
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = services.GetRequiredService<AspergillosisContext>();                  
                    var context2 = services.GetRequiredService<ApplicationDbContext>();
                    var hostingEnvironment = services.GetRequiredService<IHostingEnvironment>();
                    AspergillosisDatabaseSeeder.SeedDatabase(hostingEnvironment, context);
                    AppDbInitializer.Initialize(context2);                   
                }
                catch (Exception ex)
                {                  
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
                finally
                {                    
                }
            }
           
            host.Run();

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseUrls("http://0*:5000/")
                .UseKestrel()
                .ConfigureLogging(builder => builder.AddFile())
                .UseStartup<Startup>()
                .Build();

    }
}
