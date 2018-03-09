using AspergillosisEPR.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms.ViewModels
{
    public class CaseReportFormSectionViewModel
    {
        [Required]
        [Display(Name = "Section Name")]
        public string Name { get; set; }
        public List<CaseReportFormField> Fields { get; set; }
        public List<string> FieldNames { get; set; }
        public int ID { get; set; }
        public static void BuildSection(AspergillosisContext context, 
                                        CaseReportFormSectionViewModel formSectionVM)
        {
            var formSection = new CaseReportFormSection()
            {
                Name = formSectionVM.Name
            };
            formSection.CaseReportFormResultFields = new List<CaseReportFormField>();
            foreach(var field in formSectionVM.Fields)
            {
                formSection.CaseReportFormResultFields.Add(field);
                context.CaseReportFormFields.Add(field);
                if (field.SelectedOptionsIds == null) continue;               
                foreach (var fieldOptionId in field.SelectedOptionsIds)
                {
                    var sectionOption = new CaseReportFormFieldOption();
                    sectionOption.CaseReportFormOptionChoiceId = fieldOptionId;
                    sectionOption.Field = field;
                    context.CaseReportFormFieldOptions.Add(sectionOption);
                }
            }
            context.CaseReportFormSections.Add(formSection);
        }
    }
}
