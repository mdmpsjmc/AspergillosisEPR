using System;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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
                    DbInitializer.Initialize(context);
                    DbInitializer.AddDefaultPatientStatuses(context);
                    DbInitializer.CreateDbImportTypes(context);
                    DbInitializer.AddIgTypes(context);
                    RadiologyDataInitializer.AddRadiologyModels(context);
                    CaseReportFormsDataInitializer.AddCaseReportFormsModels(context);
                    var context2 = services.GetRequiredService<ApplicationDbContext>();
                    AppDbInitializer.Iniitalize(context2);
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
