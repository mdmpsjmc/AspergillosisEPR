using AspergillosisEPR.Models.CaseReportForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data
{
    public class CaseReportFormsDataInitializer
    {
        public static void AddCaseReportFormsModels(AspergillosisContext context)
        {
            if (context.CaseReportFormFieldTypes.Any())
            {
                return;
            }

            AddCaseResultFormDefaultFieldTypes(context);
            AddCaseResultFormDefaultOptionDropdowns(context);
            AddCaseResultFormDefaultSections(context);
            AddCaseResultFormDefaultCategories(context);

            context.SaveChanges();
        }

        public static void AddSelectFieldTypes(AspergillosisContext context)
        {
            var multiSelect = context.CaseReportFormFieldTypes
                                     .Where(ft => ft.Name.Contains("Select"));

            if (multiSelect.Any())
            {
                return;
            }

            var caseReportFormFieldTypes = new CaseReportFormFieldType[]
            {
               new CaseReportFormFieldType { Name = "Single Select"},
               new CaseReportFormFieldType { Name = "Multi Select"},
               new CaseReportFormFieldType { Name = "Check-box" },
               new CaseReportFormFieldType { Name = "Radio-button" }
            };

            foreach (var item in caseReportFormFieldTypes)
            {
                context.Add(item);
            }
            context.SaveChanges();
        }

        private static void AddCaseResultFormDefaultCategories(AspergillosisContext context)
        {
            var caseReportFormCategories = new CaseReportFormCategory[]
           {
               new CaseReportFormCategory { Name = "Mycology Serology"},
               new CaseReportFormCategory { Name = "Mycology Molecular"},
               new CaseReportFormCategory { Name = "Assay" }
           };

            foreach (var item in caseReportFormCategories)
            {
                context.CaseReportFormCategories.Add(item);
            }
        }

        private static void AddCaseResultFormDefaultSections(AspergillosisContext context)
        {
            var caseReportFormSections = new CaseReportFormSection[]
            {
               new CaseReportFormSection { Name = "Aspergillus Fumigatus Percip"},
               new CaseReportFormSection { Name = "Beta Glucan"},
               new CaseReportFormSection { Name = "Aspergillus Galactomannan ag" },
               new CaseReportFormSection { Name = "Cryptococcal Antigen" },
               new CaseReportFormSection { Name = "Expected Resistance Profile" }
            };

            foreach (var item in caseReportFormSections)
            {
                context.CaseReportFormSections.Add(item);
            }
        }

        private static void AddCaseResultFormDefaultFieldTypes(AspergillosisContext context)
        {
            var caseReportFormFieldTypes = new CaseReportFormFieldType[]
            {
               new CaseReportFormFieldType { Name = "Text Field"},
               new CaseReportFormFieldType { Name = "Numeric Field"},
               new CaseReportFormFieldType { Name = "Date Field" }
            };

            foreach (var item in caseReportFormFieldTypes)
            {
                context.Add(item);
            }
        }

        private static void AddCaseResultFormDefaultOptionDropdowns(AspergillosisContext context)
        {
            var caseReportFormOptionGroups = new CaseReportFormOptionGroup[]
            {
                new CaseReportFormOptionGroup { Name = "Specimen"},
                new CaseReportFormOptionGroup { Name = "Serology Result"}
            };

            foreach (var item in caseReportFormOptionGroups)
            {
                context.Add(item);
            }

            var caseReportFormSerologyOptionChoices = new String[] {
                "Positive",
                "Weakly Positive",
                "Negative"
            };

            var caseReportFormSpecimenOptionChoices = new String[]
            {
                "Blood","Sputum", "Fungal isolate"
            };

            var serologyOptionGroup = caseReportFormOptionGroups
                                             .Where(crfog => crfog.Name.Contains("Serology Result"))
                                             .FirstOrDefault();

            foreach (string serologyOptName in caseReportFormSerologyOptionChoices)
            {
                CaseReportFormOptionChoice serologyOpt = new CaseReportFormOptionChoice();
                serologyOpt.CaseReportFormOptionGroup = serologyOptionGroup;
                serologyOpt.Name = serologyOptName;
                context.CaseReportFormOptionChoices.Add(serologyOpt);
            }

            var specimenOptionGroup = caseReportFormOptionGroups
                                             .Where(crfog => crfog.Name.Contains("Specimen"))
                                             .FirstOrDefault();

            foreach (string serologyOptName in caseReportFormSpecimenOptionChoices)
            {
                CaseReportFormOptionChoice serologyOpt = new CaseReportFormOptionChoice();
                serologyOpt.CaseReportFormOptionGroup = specimenOptionGroup;
                serologyOpt.Name = serologyOptName;
                context.CaseReportFormOptionChoices.Add(serologyOpt);
            }
        }
    }
}
