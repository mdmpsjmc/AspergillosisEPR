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
        private CaseReportFormsDropdownResolver _resolver;

        public CaseReportFormsController(AspergillosisContext context)
        {
            _context = context;
            _resolver = new CaseReportFormsDropdownResolver(context);
        }

        public IActionResult New()
        {
            ViewBag.CategoriesIds = _resolver.PopuplateCRFCategoriesDropdownList();
            ViewBag.SectionIds = _resolver.PopulateCRFSectionsDropdownList();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create(CaseReportFormViewModel caseReportFormViewModel)
        {
            try
            {
                var caseReportForm = new CaseReportForm();
                var sections = new List<CaseReportFormFormSection>();
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
                caseReportForm.Name = caseReportFormViewModel.Name;
                caseReportForm.CaseReportFormCategoryId = caseReportFormViewModel.CaseReportFormCategoryId;
                caseReportForm.Fields = caseReportFormViewModel.Fields;
                caseReportForm.Sections = sections;

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
    }
}