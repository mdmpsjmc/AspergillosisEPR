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
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<PatientDrug> PatientDrugs { get; set; }
        public DbSet<SideEffect> SideEffects { get; set; }
        public DbSet<PatientDrugSideEffect> PatientDrugSideEffects { get; set; }
        public DbSet<PatientStatus> PatientStatuses { get; set; }
        public DbSet<DbImport> DbImports { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().ToTable("Patients");
            modelBuilder.Entity<PatientDiagnosis>().ToTable("PatientDiagnosis");
            modelBuilder.Entity<DiagnosisType>().ToTable("DiagnosisTypes");
            modelBuilder.Entity<DiagnosisCategory>().ToTable("DiagnosisCategories");
            modelBuilder.Entity<Drug>().ToTable("Drugs");
            modelBuilder.Entity<PatientDrug>().ToTable("PatientDrugs");
            modelBuilder.Entity<SideEffect>().ToTable("SideEffects");
            modelBuilder.Entity<PatientDrugSideEffect>().ToTable("PatientDrugSideEffects");
            modelBuilder.Entity<PatientStatus>().ToTable("PatientStatuses");
            modelBuilder.Entity<DbImport>().ToTable("DbImports");
        }
    }


}