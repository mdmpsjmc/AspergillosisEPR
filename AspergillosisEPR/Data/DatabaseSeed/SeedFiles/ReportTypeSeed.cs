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


        public static void AddCPAMortalityAudit(AspergillosisContext context)
        {
            var report = context.ReportTypes.Where(rt => rt.Code == "cpa-mortality-report").FirstOrDefault();
            if (report == null)
            {
                var newReport = new ReportType {
                        Name = "CPA Mortality Audit",
                        Discriminator = "CPAMortalityAudit",
                        Code = "cpa-mortality-report"
                };
                context.ReportTypes.Add(newReport);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }

        public static void AddIgGAndSGRQReport(AspergillosisContext context)
        {
            var report = context.ReportTypes.Where(rt => rt.Code == "igg-sgrq-report").FirstOrDefault();
            if (report == null)
            {
                var newReport = new ReportType
                {
                    Name = "IgG and SGRQ Report",
                    Discriminator = "SGRQandIgReportType",
                    Code = "igg-sgrq-report"
                };
                context.ReportTypes.Add(newReport);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }
    }
}
