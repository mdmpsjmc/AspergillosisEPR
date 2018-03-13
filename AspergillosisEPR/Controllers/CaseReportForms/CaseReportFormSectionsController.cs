using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.CaseReportForms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Helpers;
using System.Collections;
using AspergillosisEPR.Models.CaseReportForms.ViewModels;
using AspergillosisEPR.Models.CaseReportForms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspergillosisEPR.Controllers.CaseReportForms
{
    public class CaseReportFormSectionsController : Controller
    {
        private AspergillosisContext _context;
        private CaseReportFormsDropdownResolver _resolver;

        public CaseReportFormSectionsController(AspergillosisContext context)
        {
            _context = context;
            _resolver = new CaseReportFormsDropdownResolver(context);
        }

        public IActionResult New()
        {
            ViewBag.FieldTypes = _resolver.PopulateCRFFieldTypesDropdownList();
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormSections/New.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create(CaseReportFormSectionViewModel formSectionVM)
        {
            if (ModelState.IsValid)
            {
                CaseReportFormSectionViewModel.BuildSection(_context, formSectionVM);
                await _context.SaveChangesAsync();
                return Json(new { result = "ok" });
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
        }

        public IActionResult Show(int? id)
        {
            ViewBag.FieldTypes = _resolver.PopulateCRFFieldTypesDropdownList();
            if (id == null)
            {
                return NotFound();
            }
            var section = ObtainFormSection(id);

            if (section == null)
            {
                return NotFound();
            }
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormSections/Show.cshtml", section);
        }

        public IActionResult Edit(int? id)
        {
            ViewBag.FieldTypes = _resolver.PopulateCRFFieldTypesDropdownList();
            if (id == null)
            {
                return NotFound();
            }
            var section = ObtainFormSection(id);

            if (section == null)
            {
                return NotFound();
            }
            var fields = section.CaseReportFormResultFields.OrderBy(f => f.ID).ToList();
            ViewBag.OptionGroupsIds = new List<SelectList>();
            ViewBag.FieldTypeIds = new List<SelectList>();
            ViewBag.FieldOptions = new List<MultiSelectList>();

            for (int cursor = 0; cursor < fields.Count(); cursor++)
            {
                var field = fields[cursor];
                var optionGroupId = field.Options.FirstOrDefault()?.Option?.CaseReportFormOptionGroupId;
                ViewBag.OptionGroupsIds.Add(_resolver.PopulateCRFOptionGroupsDropdownList(optionGroupId));
                ViewBag.FieldTypeIds.Add(_resolver.PopulateCRFFieldTypesDropdownList(field.CaseReportFormFieldTypeId));
                _context.Entry(field).Collection(m => m.Options).Load();
                if (optionGroupId != null)
                {
                    var selectedIds = field.Options.Select(o => o.CaseReportFormOptionChoiceId).ToList();
                    ViewBag.FieldOptions.Add(_resolver.PopulateCRFOptionGroupChoicesDropdownList(optionGroupId.Value, selectedIds));
                } else
                {
                    ViewBag.FieldOptions.Add(new SelectList(new List<int>()));
                }
            }

            return PartialView(@"~/Views/CaseReportForms/CaseReportFormSections/Edit.cshtml", section);
        }

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin Role, Update Role")]
        [ValidateAntiForgeryToken]
        public IActionResult EditFormSection(int? id,
                                             CaseReportFormSection section,
                                             CaseReportFormField[] fields)
        {
            section.CaseReportFormResultFields = fields;
            foreach(var field in section.CaseReportFormResultFields.OrderBy(f => f.ID))
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
                _context.Update(section);
                _context.SaveChanges();
                return Json(new { valid = true });
            } else
            {
                var errors = ModelStateHelper.Errors(ModelState);
                return Json(new {errors = errors, valid = false });
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin Role, Delete Role")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            var item = _context.CaseReportFormSections.SingleOrDefault(fs => fs.ID == id);           
            var sectionFields = _context.CaseReportFormFields.Where(f => f.CaseReportFormSectionId == id);
            _context.RemoveRange(sectionFields);
            _context.CaseReportFormSections.Remove(item);
            _context.SaveChanges();
            return Json(new { ok = "ok" });
        }

        private CaseReportFormSection ObtainFormSection(int? id)
        {
            return _context.CaseReportFormSections
                                  .Where(crfs => crfs.ID == id)
                                  .Include(crfs => crfs.CaseReportFormResultFields)
                                     .ThenInclude(f => f.CaseReportFormFieldType)
                                  .Include(crfs => crfs.CaseReportFormResultFields)
                                     .ThenInclude(f => f.Options)
                                        .ThenInclude(o => o.Option)
                                  .FirstOrDefault();
        }
    }
}