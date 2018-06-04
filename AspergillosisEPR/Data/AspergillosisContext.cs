using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using AspergillosisEPR.Extensions;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;
using AspergillosisEPR.Models.CaseReportForms;
using AspergillosisEPR.Models.CaseReportForms.ViewModels;
using AspergillosisEPR.Models.Radiology;
using AspergillosisEPR.Models.Patients;
using AspergillosisEPR.Models.MedicalTrials;

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
        public DbSet<PatientSTGQuestionnaire> PatientSTGQuestionnaires { get; set; }
        public DbSet<DbImportType> DBImportTypes { get; set; }
        public DbSet<ImmunoglobulinType> ImmunoglobulinTypes { get; set; }
        public DbSet<PatientImmunoglobulin> PatientImmunoglobulins { get; set; }
        public DbSet<Finding> Findings { get; set; }
        public DbSet<ChestLocation> ChestLocations { get; set; }
        public DbSet<ChestDistribution> ChestDistributions { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<TreatmentResponse> TreatmentResponses { get; set; }
        public DbSet<RadiologyType> RadiologyTypes { get; set; }
        public DbSet<PatientRadiologyFinding> PatientRadiologyFindings { get; set; }
        public DbSet<PatientExamination> PatientExaminations { get; set; }
        public DbSet<SGRQExamination> SGRQExaminations { get; set; }
        public DbSet<ImmunologyExamination> ImmunologyExaminations { get; set; }
        public DbSet<RadiologyExamination> RadiologyExaminations { get; set; }
        public DbSet<PatientVisit> PatientVisits { get; set; }
        public DbSet<PatientMeasurement> PatientMeasurements { get; set; }
        public DbSet<MeasurementExamination> MeasurementExaminations { get; set; }

        public DbSet<CaseReportForm> CaseReportForms { get; set; }
        public DbSet<CaseReportFormField> CaseReportFormFields { get; set; }
        public DbSet<CaseReportFormFieldType> CaseReportFormFieldTypes { get; set; }
        public DbSet<CaseReportFormOptionChoice> CaseReportFormOptionChoices { get; set; }
        public DbSet<CaseReportFormOptionGroup> CaseReportFormOptionGroups { get; set; }
        public DbSet<CaseReportFormSection> CaseReportFormSections { get; set; }
        public DbSet<CaseReportFormCategory> CaseReportFormCategories { get; set; }
        public DbSet<CaseReportFormFieldOption> CaseReportFormFieldOptions { get; set; }
        public DbSet<CaseReportFormFormSection> CaseReportFormFormSections { get; set; }
        public DbSet<CaseReportFormPatientResult> CaseReportFormPatientResults { get; set; }
        public DbSet<CaseReportFormPatientResultOptionChoice> CaseReportFormPatientResultOptionChoices { get; set; }
        public DbSet<CaseReportFormResult> CaseReportFormResults { get; set; }

        public DbSet<PersonTitle> PersonTitles { get; set; }
        public DbSet<MedicalTrial> MedicalTrials { get; set; }
        public DbSet<PatientMedicalTrial> PatientMedicalTrials { get; set; }
        public DbSet<MedicalTrialPatientStatus> MedicalTrialPatientStatuses { get; set; }
        public DbSet<MedicalTrialPrincipalInvestigator> MedicalTrialsPrincipalInvestigators { get; set; }
        public DbSet<MedicalTrialStatus> MedicalTrialStatuses { get; set; }
        public DbSet<MedicalTrialType> MedicalTrialTypes { get; set; }

        public DbSet<PatientDrugLevel> PatientDrugLevels { get; set; }
        public DbSet<UnitOfMeasurement> UnitOfMeasurements { get; set; }
        public DbSet<TemporaryNewPatient> TemporaryNewPatient { get; set; }

        public DbSet<PulmonaryFunctionTest> PulmonaryFunctionTests { get; set; }


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
            modelBuilder.Entity<PatientSTGQuestionnaire>().ToTable("PatientSTGQuestionnaires");
            modelBuilder.Entity<PatientSTGQuestionnaire>().Property(x => x.ImpactScore).HasPrecision(10, 2);
            modelBuilder.Entity<PatientSTGQuestionnaire>().Property(x => x.ActivityScore).HasPrecision(10, 2);
            modelBuilder.Entity<PatientSTGQuestionnaire>().Property(x => x.TotalScore).HasPrecision(10, 2);
            modelBuilder.Entity<PatientSTGQuestionnaire>().Property(x => x.SymptomScore).HasPrecision(10, 2);
            modelBuilder.Entity<DbImportType>().ToTable("DbImportTypes");
            modelBuilder.Entity<ImmunoglobulinType>().ToTable("ImmunoglobulinTypes");
            modelBuilder.Entity<PatientImmunoglobulin>().ToTable("PatientImmunoglobulins");
            modelBuilder.Entity<Finding>().ToTable("Findings");
            modelBuilder.Entity<ChestLocation>().ToTable("ChestLocations");
            modelBuilder.Entity<ChestDistribution>().ToTable("ChestDistributions");
            modelBuilder.Entity<Grade>().ToTable("Grades");
            modelBuilder.Entity<TreatmentResponse>().ToTable("TreatmentResponses");
            modelBuilder.Entity<RadiologyType>().ToTable("RadiologyTypes");
            modelBuilder.Entity<PatientRadiologyFinding>().ToTable("PatientRadiologyFindings");
            modelBuilder.Entity<PatientVisit>().ToTable("PatientVisits");
            modelBuilder.Entity<PatientMeasurement>().ToTable("PatientMeasurements");

            modelBuilder.Entity<CaseReportFormField>().ToTable("CaseReportFormFields");
            modelBuilder.Entity<CaseReportFormFieldType>().ToTable("CaseReportFormFieldTypes");
            modelBuilder.Entity<CaseReportFormOptionChoice>().ToTable("CaseReportFormOptionChoices");
            modelBuilder.Entity<CaseReportFormOptionGroup>().ToTable("CaseReportFormOptionGroups");
            modelBuilder.Entity<CaseReportFormSection>().ToTable("CaseReportFormSections");
            modelBuilder.Entity<CaseReportFormCategory>().ToTable("CaseReportFormCategories");
            modelBuilder.Entity<CaseReportForm>().ToTable("CaseReportForms");
            modelBuilder.Entity<CaseReportFormFieldOption>().ToTable("CaseReportFormFieldOptions");
            modelBuilder.Entity<CaseReportFormFormSection>().ToTable("CaseReportFormFormSections");
            modelBuilder.Entity<CaseReportFormPatientResult>().ToTable("CaseReportFormPatientResults");
            modelBuilder.Entity<CaseReportFormPatientResultOptionChoice>().ToTable("CaseReportFormPatientResultOptionChoices");
            modelBuilder.Entity<CaseReportFormResult>().ToTable("CaseReportFormResults");

            modelBuilder.Entity<PersonTitle>().ToTable("PersonTitles");
            modelBuilder.Entity<MedicalTrial>().ToTable("MedicalTrials");
            modelBuilder.Entity<PatientMedicalTrial>().ToTable("PatientMedicalTrials");
            modelBuilder.Entity<MedicalTrialPatientStatus>().ToTable("MedicalTrialPatientStatuses");
            modelBuilder.Entity<MedicalTrialPrincipalInvestigator>().ToTable("MedicalTrialPrincipalInvestigators");
            modelBuilder.Entity<MedicalTrialStatus>().ToTable("MedicalTrialStatuses");
            modelBuilder.Entity<MedicalTrialType>().ToTable("MedicalTrialTypes");
            modelBuilder.Entity<PatientDrugLevel>().ToTable("PatientDrugLevel");
            modelBuilder.Entity<UnitOfMeasurement>().ToTable("UnitOfMeasurements");
            modelBuilder.Entity<TemporaryNewPatient>().ToTable("TemporaryNewPatients");
        }

    }


}