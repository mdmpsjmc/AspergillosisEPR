using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Data
{
    public class AspergillosisContext : DbContext
    {
        public AspergillosisContext(DbContextOptions<AspergillosisContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<DiagnosisType> DiagnosisTypes { get; set; }
        public DbSet<PatientDiagnosis> PatientDiagnoses { get; set; }
        public DbSet<DiagnosisCategory> DiagnosisCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().ToTable("Patients");
            modelBuilder.Entity<PatientDiagnosis>().ToTable("PatientDiagnosis");
            modelBuilder.Entity<DiagnosisType>().ToTable("DiagnosisTypes");
            modelBuilder.Entity<DiagnosisCategory>().ToTable("DiagnosisCategories");

        }
    }


}