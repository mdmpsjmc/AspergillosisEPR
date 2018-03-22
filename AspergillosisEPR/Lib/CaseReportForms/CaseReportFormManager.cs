using AspergillosisEPR.Data;
using AspergillosisEPR.Models.CaseReportForms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Lib.CaseReportForms
{
    public class CaseReportFormManager
    {
        private AspergillosisContext _context;

        public CaseReportFormManager(AspergillosisContext context)
        {
            _context = context;
        }

        public void UpdateCaseReportFormsForPatient(CaseReportFormResult[] caseReportFormResult,
                                                    Patient patientToUpdate)
        {
            if (caseReportFormResult != null)
            {
                patientToUpdate.CaseReportFormResults = new List<CaseReportFormResult>();
                foreach (var result in caseReportFormResult)
                {
                    if (result.Results != null)
                    {
                        if (result.ID == 0)
                        {
                            AddNewResultForPatient(patientToUpdate, result);
                        }
                        else
                        {
                            UpdateResultForPatient(patientToUpdate, result);
                        }
                    }
                }
            }
        }
      
        public void UpdateOptionChoices(CaseReportFormPatientResult[] caseReportFormPatientResult)
        {
            for (int cursor = 0; cursor < caseReportFormPatientResult.Length; cursor++)
            {
                var caseReportFormResult = caseReportFormPatientResult[cursor];
                if (caseReportFormResult.SelectedIds != null || caseReportFormResult.SelectedId != null)
                {
                    
                    if (caseReportFormResult.SelectedId != null)
                    {
                        var optionChoice = _context.CaseReportFormOptionChoices
                                                   .Where(oc => oc.ID == caseReportFormResult.SelectedId)
                                                   .FirstOrDefault();
                        if (optionChoice == null) continue;
                        caseReportFormResult.Options = new List<CaseReportFormPatientResultOptionChoice>();
                        var caseReportResultOptionChoice = new CaseReportFormPatientResultOptionChoice();
                        caseReportResultOptionChoice.CaseReportFormOptionChoiceId = optionChoice.ID;
                        caseReportResultOptionChoice.CaseReportFormPatientResultId = caseReportFormResult.ID;
                        caseReportFormResult.Options.Add(caseReportResultOptionChoice);
                        _context.Add(caseReportResultOptionChoice);
                    }

                    if (caseReportFormResult.SelectedIds != null)
                    {
                
                        foreach (var optionChoiceId in caseReportFormResult.SelectedIds)
                        {                            
                            var optionChoice = _context.CaseReportFormOptionChoices
                                                   .Where(oc => oc.ID == optionChoiceId)
                                                   .FirstOrDefault();
                            if (optionChoice == null) continue;
                            caseReportFormResult.Options = new List<CaseReportFormPatientResultOptionChoice>();
                            var caseReportResultOptionChoice = new CaseReportFormPatientResultOptionChoice();
                            caseReportResultOptionChoice.CaseReportFormOptionChoiceId = optionChoice.ID;
                            caseReportResultOptionChoice.CaseReportFormPatientResultId = caseReportFormResult.ID;
                            caseReportFormResult.Options.Add(caseReportResultOptionChoice);
                            _context.Add(caseReportResultOptionChoice);
                        }
                    }
                }
            }
        }

        public CaseReportForm FindByIdWithAllRelations(int id)
        {
            return _context.CaseReportForms
                                         .Where(f => f.ID == id)
                                         .Include(f => f.CaseReportFormCategory)
                                         .Include(f => f.Fields)
                                            .ThenInclude(f => f.CaseReportFormFieldType)
                                         .Include(f => f.Fields)
                                            .ThenInclude(f => f.Options)
                                                .ThenInclude(o => o.Option)
                                         .Include(f => f.Sections)
                                            .ThenInclude(s => s.Section)
                                                .ThenInclude(s => s.CaseReportFormResultFields)
                                                        .ThenInclude(f => f.Options)
                                                            .ThenInclude(o => o.Option)
                                         .Include(f => f.Sections)
                                            .ThenInclude(s => s.Section)
                                                .ThenInclude(s => s.CaseReportFormResultFields)
                                                    .ThenInclude(f => f.CaseReportFormFieldType)
                                         .FirstOrDefault();
        }


        public void BuildFormFor(dynamic viewBag, 
                                 List<CaseReportFormField> fields,
                                 CaseReportFormsDropdownResolver resolver)
        {

            viewBag.OptionGroupsIds = new List<SelectList>();
            viewBag.FieldTypeIds = new List<SelectList>();
            viewBag.FieldOptions = new List<MultiSelectList>();

            for (int cursor = 0; cursor < fields.Count(); cursor++)
            {
                var field = fields[cursor];
                var optionGroupId = field.Options.FirstOrDefault()?.Option?.CaseReportFormOptionGroupId;
                viewBag.OptionGroupsIds.Add(resolver.PopulateCRFOptionGroupsDropdownList(optionGroupId));
                viewBag.FieldTypeIds.Add(resolver.PopulateCRFFieldTypesDropdownList(field.CaseReportFormFieldTypeId));
                _context.Entry(field).Collection(m => m.Options).Load();
                if (optionGroupId != null)
                {
                    var selectedIds = field.Options.Select(o => o.CaseReportFormOptionChoiceId).ToList();
                    viewBag.FieldOptions.Add(resolver.PopulateCRFOptionGroupChoicesDropdownList(optionGroupId.Value, selectedIds));
                }
                else
                {
                    viewBag.FieldOptions.Add(new SelectList(new List<int>()));
                }
            }
        }

        public List<IGrouping<string, CaseReportFormResult>> GetGroupedCaseReportFormsForPatient(int patientId)
        {
            var forms = _context.CaseReportFormResults
                                .Where(pr => pr.PatientId == patientId)
                                .Include(fr => fr.Results)
                                    .ThenInclude(pr => pr.Options)
                                .Include(fr => fr.Results)
                                    .ThenInclude(pr => pr.Options)
                                .Include(fr => fr.Results)
                                .ThenInclude(pr => pr.FormResult)
                                    .ThenInclude(f => f.CaseReportFormCategory)
                                .Include(fr => fr.Results)
                                .ThenInclude(pr => pr.FormResult)
                                    .ThenInclude(f => f.Fields)
                                       .ThenInclude(f => f.CaseReportFormFieldType)
                                .Include(fr => fr.Results)
                                  .ThenInclude(pr => pr.FormResult)
                                    .ThenInclude(f => f.Fields)
                                        .ThenInclude(f => f.Options)
                                            .ThenInclude(o => o.Option)
                                .Include(fr => fr.Results)
                                  .ThenInclude(pr => pr.FormResult)
                                      .ThenInclude(f => f.Sections)
                                            .ThenInclude(s => s.Section)
                                                .ThenInclude(s => s.CaseReportFormResultFields)
                                                        .ThenInclude(f => f.Options)
                                                            .ThenInclude(o => o.Option)
                                .Include(fr => fr.Results)
                                  .ThenInclude(pr => pr.FormResult)
                                         .ThenInclude(f => f.Sections)
                                            .ThenInclude(s => s.Section)
                                                .ThenInclude(s => s.CaseReportFormResultFields)
                                                    .ThenInclude(f => f.CaseReportFormFieldType)
                                .Include(fr => fr.Results)
                                .ThenInclude(pr => pr.FormResult)
                                    .ThenInclude(f => f.Fields)
                                        .ThenInclude(f => f.CaseReportForm);

            var grouped = forms.GroupBy( f=> f.Category.Name).ToList();
            return grouped;
        }

        public void GetFormIdsForCaseReportForms(CaseReportFormPatientResult[] caseReportFormPatientResult)
        {
            for(int cursor = 0; cursor < caseReportFormPatientResult.Length; cursor++)
            {
                var caseReportFormResult = caseReportFormPatientResult[cursor];
                var field = _context.CaseReportFormFields.Where(f => f.ID == caseReportFormResult.CaseReportFormFieldId)
                                                         .Include(f => f.CaseReportFormSection)
                                                         .Include(f => f.CaseReportForm)
                                                         .FirstOrDefault();

                int? formId = null;
                if (field == null)
                {
                   
                }
                else
                {
                    formId = (field.CaseReportForm != null) ? field.CaseReportFormId : GetSectionFormIdForField(field);
                }
                caseReportFormResult.CaseReportFormId = formId.Value;
            }
        }

        public void UpdateWithPatient(Patient patient,
                                     CaseReportFormPatientResult[] caseReportFormPatientResult)
        {
            for (int cursor = 0; cursor < caseReportFormPatientResult.Length; cursor++)
            {
                var caseReportFormResult = caseReportFormPatientResult[cursor];
                caseReportFormResult.Patient = patient;
            }
        }

        public void CreateOptionChoices(CaseReportFormPatientResult[] caseReportFormPatientResult)
        {
            for (int cursor = 0; cursor < caseReportFormPatientResult.Length; cursor++)
            {
                var caseReportFormResult = caseReportFormPatientResult[cursor];
                if (caseReportFormResult.SelectedIds != null || caseReportFormResult.SelectedId != null)
                {
                    caseReportFormResult.Options = new List<CaseReportFormPatientResultOptionChoice>();
                    if (caseReportFormResult.SelectedId != null)
                    {
                        var optionChoice = _context.CaseReportFormOptionChoices
                                                   .Where(oc => oc.ID == caseReportFormResult.SelectedId)
                                                   .FirstOrDefault();
                        if (optionChoice == null) continue;
                        var caseReportResultOptionChoice = new CaseReportFormPatientResultOptionChoice();
                        caseReportResultOptionChoice.CaseReportFormOptionChoiceId = optionChoice.ID;
                        caseReportFormResult.Options.Add(caseReportResultOptionChoice);
                    }

                    if (caseReportFormResult.SelectedIds != null)
                    {
                        foreach(var optionChoiceId in caseReportFormResult.SelectedIds)
                        {
                            var optionChoice = _context.CaseReportFormOptionChoices
                                                   .Where(oc => oc.ID == optionChoiceId)
                                                   .FirstOrDefault();
                            if (optionChoice == null) continue;
                            var caseReportResultOptionChoice = new CaseReportFormPatientResultOptionChoice();
                            caseReportResultOptionChoice.CaseReportFormOptionChoiceId = optionChoice.ID;
                            caseReportFormResult.Options.Add(caseReportResultOptionChoice);
                        }
                    }
                }
            }
        }


        private int? GetSectionFormIdForField(CaseReportFormField field)
        {
            if (field.CaseReportFormSectionId != null)
            {
                var section = _context.CaseReportFormFormSections
                                               .Where(s => s.CaseReportFormSectionId == field.CaseReportFormSectionId)
                                               .FirstOrDefault();
                return section.CaseReportFormId;
            }
            else
            {
                return null;
            }
        }

        private void UpdateResultForPatient(Patient patientToUpdate, CaseReportFormResult caseReportFormResult)
        {
            var results = caseReportFormResult.Results.ToArray();
            GetFormIdsForCaseReportForms(results);
            UpdateWithPatient(patientToUpdate, results);
            foreach (var singleItemResult in results)
            {
                UpdateItemResultInDatabase(patientToUpdate, caseReportFormResult, singleItemResult);
            }
        }

        private void UpdateItemResultInDatabase(Patient patientToUpdate,
                                                CaseReportFormResult caseReportFormResult,
                                                CaseReportFormPatientResult singleItemResult)
        {
            var dbItemResult = _context.CaseReportFormPatientResults
                                                       .Where(pr => pr.PatientId == patientToUpdate.ID
                                                              && pr.CaseReportFormFieldId == singleItemResult.CaseReportFormFieldId
                                                              && pr.CaseReportFormId == caseReportFormResult.CaseReportFormId
                                                              && pr.CaseReportFormResultId == caseReportFormResult.ID)
                                                       .FirstOrDefault();
            if (dbItemResult != null)
            {
                dbItemResult.CaseReportFormFieldId = singleItemResult.CaseReportFormFieldId;
                dbItemResult.CaseReportFormResultId = caseReportFormResult.ID;
                dbItemResult.NumericAnswer = singleItemResult.NumericAnswer;
                dbItemResult.TextAnswer = singleItemResult.TextAnswer;
                dbItemResult.SelectedId = singleItemResult.SelectedId;
                dbItemResult.SelectedIds = singleItemResult.SelectedIds;
                dbItemResult.DateAnswer = singleItemResult.DateAnswer;
                _context.Update(dbItemResult);
            }
        }

        private void AddNewResultForPatient(Patient patientToUpdate, 
                                           CaseReportFormResult result)
        {
            GetFormIdsForCaseReportForms(result.Results.ToArray());
            UpdateWithPatient(patientToUpdate, result.Results.ToArray());
            UpdateOptionChoices(result.Results.ToArray());
            result.PatientId = patientToUpdate.ID;
            patientToUpdate.CaseReportFormResults.Add(result);
            _context.Update(result);
        }

    }
}
