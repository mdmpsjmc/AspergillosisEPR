using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.CaseReportForms;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models.CaseReportForms.ViewModels;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.CaseReportForms;

namespace AspergillosisEPR.Controllers.CaseReportForms
{
    public class CaseReportFormsController : Controller
    {
        private AspergillosisContext _context;
        private CaseReportFormsDropdownResolver _dropdownResolver;
        private CaseReportFormManager _caseReportFormResolver;

        public CaseReportFormsController(AspergillosisContext context)
        {
            _context = context;
            _dropdownResolver = new CaseReportFormsDropdownResolver(context);
            _caseReportFormResolver = new CaseReportFormManager(context);
        }

        public IActionResult New()
        {
            ViewBag.CategoriesIds = _dropdownResolver.PopuplateCRFCategoriesDropdownList();
            ViewBag.SectionIds = _dropdownResolver.PopulateCRFSectionsDropdownList();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create(CaseReportFormViewModel caseReportFormViewModel)
        {
            try
            {
                CaseReportForm caseReportForm;
                List<CaseReportFormFormSection> sections;
                BuildFormWithSections(caseReportFormViewModel, out caseReportForm, out sections);

                if (caseReportFormViewModel.SectionsIds == null && caseReportFormViewModel.Fields == null)
                {
                    ModelState.AddModelError("Base",
                                             "Add least one section or field needs to be present on any form");
                }

                if (ModelState.IsValid)
                {
                    _context.CaseReportFormFormSections.AddRange(sections);
                    _context.CaseReportForms.Add(caseReportForm);
                    await _context.SaveChangesAsync();
                    return Json(new { result = "ok" });
                }
                else
                {
                    Hashtable errors = ModelStateHelper.Errors(ModelState);
                    return Json(new { success = false, errors });
                }
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }        

        public IActionResult Show(int? id)
        {
            var caseReportForm = _caseReportFormResolver.FindByIdWithAllRelations(id.Value);

            if (caseReportForm == null)
            {
                return NotFound();
            }
            var viewModel = CaseReportFormViewModel.BuildViewModel(caseReportForm);
            return PartialView(@"/Views/CaseReportForms/_Show.cshtml", viewModel);
        }

        public IActionResult Edit(int? id)
        {
            var caseReportForm = _caseReportFormResolver.FindByIdWithAllRelations(id.Value);

            if (caseReportForm == null)
            {
                return NotFound();
            }
            var viewModel = CaseReportFormViewModel.BuildViewModel(caseReportForm);
            var sectionIds = caseReportForm.Sections.Select(s => s.CaseReportFormSectionId).ToList();

            ViewBag.CategoriesIds = _dropdownResolver.PopuplateCRFCategoriesDropdownList(caseReportForm.CaseReportFormCategoryId);
            ViewBag.SectionIds = _dropdownResolver.PopulateCRFSectionsDropdownList(sectionIds);
            _caseReportFormResolver.BuildFormFor(ViewBag, caseReportForm.Fields.ToList(), _dropdownResolver);

            return PartialView(@"/Views/CaseReportForms/_Edit.cshtml", viewModel);
        }


        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin Role, Update Role")]
        [ValidateAntiForgeryToken]
        public IActionResult EditForm(int? id, CaseReportForm caseReportForm,
                                               CaseReportFormField[] fields)
        {
            caseReportForm.Fields = fields;
            foreach(var field in caseReportForm.Fields.OrderBy(f => f.ID))
            {

                var dbOptionsIds = _context.CaseReportFormFieldOptions.Where(fo => fo.CaseReportFormFieldId == field.ID).Select(fo => fo.CaseReportFormOptionChoiceId).ToList();
                var formIds = new List<int>();
                if (field.SelectedOptionsIds != null)
                {
                    formIds = field.SelectedOptionsIds.ToList();
                }
                var toDeleteOptions = dbOptionsIds.Except(formIds);
                var toInsertOptions = formIds.Except(dbOptionsIds);
                if (toDeleteOptions.Count() > 0)
                {
                    var fieldOptions = _context.CaseReportFormFieldOptions.Where(fo => fo.CaseReportFormFieldId == field.ID
                                                                                 && toDeleteOptions.Contains(fo.CaseReportFormOptionChoiceId));
                    _context.RemoveRange(fieldOptions);
                }
                if (toInsertOptions.Count() > 0)
                {
                    foreach(var optionId in toInsertOptions)
                    {
                        var fieldOption = new CaseReportFormFieldOption();
                        fieldOption.CaseReportFormFieldId = field.ID;
                        fieldOption.CaseReportFormOptionChoiceId = optionId;
                        _context.Add(fieldOption);
                    }
                }
            }
            if (ModelState.IsValid)
            {
                _context.Update(caseReportForm);
                _context.SaveChanges();
                return Json(new { valid = true });
            } else
            {
                var errors = ModelStateHelper.Errors(ModelState);
                return Json(new {errors = errors, valid = false });
            }
        }

        private void BuildFormWithSections(CaseReportFormViewModel caseReportFormViewModel, 
                                           out CaseReportForm caseReportForm, 
                                           out List<CaseReportFormFormSection> sections)
        {
            caseReportForm = new CaseReportForm();
            sections = new List<CaseReportFormFormSection>();
            caseReportForm.Fields = new List<CaseReportFormField>();
            if (caseReportFormViewModel.SectionsIds != null)
            {
                foreach (var sectionId in caseReportFormViewModel.SectionsIds)
                {
                    var section = _context.CaseReportFormSections
                                          .Where(s => s.ID == sectionId)
                                          .SingleOrDefault();
                    var formSection = new CaseReportFormFormSection();
                    formSection.CaseReportFormSectionId = sectionId;
                    sections.Add(formSection);

                }
            }
            if (caseReportFormViewModel.Fields != null)
            {
                foreach (var field in caseReportFormViewModel.Fields)
                {
                    if (field.SelectedOptionsIds == null) continue;
                    foreach (var fieldOptionId in field.SelectedOptionsIds)
                    {
                        var sectionOption = new CaseReportFormFieldOption();
                        sectionOption.CaseReportFormOptionChoiceId = fieldOptionId;
                        sectionOption.Field = field;
                        _context.CaseReportFormFieldOptions.Add(sectionOption);
                    }
                }
            }
            caseReportForm.Name = caseReportFormViewModel.Name;
            caseReportForm.CaseReportFormCategoryId = caseReportFormViewModel.CaseReportFormCategoryId;
            caseReportForm.Fields = caseReportFormViewModel.Fields;
            caseReportForm.Sections = sections;
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin Role, Delete Role")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            var item = _context.CaseReportForms.SingleOrDefault(f => f.ID == id);
            var formFields = _context.CaseReportFormFields.Where(f => f.CaseReportFormId == id);
            _context.RemoveRange(formFields);
            _context.CaseReportForms.Remove(item);
            _context.SaveChanges();
            return Json(new { ok = "ok" });
        }

        public IActionResult Patient(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseReportForm = _caseReportFormResolver.FindByIdWithAllRelations(id.Value);

            if (caseReportForm == null)
            {
                return NotFound();
            }
            var caseReportFormResult = new CaseReportFormResult();
            caseReportFormResult.CaseReportFormId = id.Value;
            caseReportFormResult.Form = caseReportForm;
            caseReportFormResult.CaseReportFormCategoryId = caseReportForm.CaseReportFormCategoryId;
            ViewBag.Index = Request.Query["index"] == "undefined" ? "0" : Request.Query["index"].ToString();
            return PartialView(@"/Views/Patients/CaseReportForms/_Show.cshtml", caseReportFormResult);
        }
    }
}