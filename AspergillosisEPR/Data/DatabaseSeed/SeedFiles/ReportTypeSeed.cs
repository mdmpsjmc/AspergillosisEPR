using AspergillosisEPR.Models.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class ReportTypeSeed
    {
        public static void Initialize(AspergillosisContext context)
        {
            context.Database.EnsureCreated();
            if (context.ReportTypes.Any())
            {
                return;
            }

            var reportTypes = new ReportType[]
            {
                new ReportType()
                {
                    Name="SGRQ Report",
                    Code="sgrq-report",
                    Discriminator = "SGRQReportType"
                },
                new ReportType()
                {
                    Name="Patient Heatmap Report",
                    Code="heatmap-report",
                    Discriminator = "PatientHeatmapReportType"
                }
            };
            foreach (ReportType r in reportTypes)
            {
                context.ReportTypes.Add(r);
            }
            context.SaveChanges();
        }
    }
}
