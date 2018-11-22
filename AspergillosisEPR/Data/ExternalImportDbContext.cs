using AspergillosisEPR.Models;
using AspergillosisEPR.Models.ExternalImportDb;
using AspergillosisEPR.Models.PatientAdministrationSystem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data
{
    public class ExternalImportDbContext : DbContext
    {
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<ExternalPatient> Patients { get; set; }
        public DbSet<PathologyReport> PathologyReports { get; set; }

        public ExternalImportDbContext(DbContextOptions<ExternalImportDbContext> options) : base(options)
        {
        }
    }
}
