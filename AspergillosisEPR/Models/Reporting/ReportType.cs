using AspergillosisEPR.Data.DatabaseSeed.SeedFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Reporting
{
    public class ReportType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Discriminator { get; set; }

        public ICollection<Report> Reports { get; set; }
    }
}
