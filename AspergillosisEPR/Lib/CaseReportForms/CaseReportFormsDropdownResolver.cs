using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.CaseReportForms
{
    public class CaseReportFormsDropdownResolver
    {
        private AspergillosisContext _context;

        public CaseReportFormsDropdownResolver(AspergillosisContext context)
        {
            _context = context;
        }

        public SelectList PopulateCRFFieldTypesDropdownList(object selectedItem = null)
        {
            var statuses = from se in _context.CaseReportFormFieldTypes
                           orderby se.ID
                           select se;
            return new SelectList(statuses, "ID", "Name", selectedItem);
        }

        public MultiSelectList PopulateCRFSectionsDropdownList(IList selectedItems = null)
        {
            var sections = _context.CaseReportFormSections
                               .OrderBy(fs => fs.Name)
                               .ToList();
            return new MultiSelectList(sections, "ID", "Name", selectedItems);
        }

        public SelectList PopuplateCRFCategoriesDropdownList(object selectedItem = null)
        {
            var categories = from se in _context.CaseReportFormCategories
                             orderby se.Name
                             select se;
            return new SelectList(categories, "ID", "Name", selectedItem);
        }

        public MultiSelectList PopulateCRFOptionGroupChoicesDropdownList(int id,
                                                                    IList selectedItems = null)
        {
            var options = _context.CaseReportFormOptionChoices
                                  .Where(crfoc => crfoc.CaseReportFormOptionGroupId == id)
                                  .ToList();
            return new MultiSelectList(options, "ID", "Name", selectedItems);
        }

        public SelectList PopulateCRFOptionGroupsDropdownList(object selectedItem = null)
        {
            var statuses = from se in _context.CaseReportFormOptionGroups
                           orderby se.Name
                           select se;
            return new SelectList(statuses, "ID", "Name", selectedItem);
        }

        public List<SelectListItem> PopulateCRFGroupedCategoriesDropdownList()
        {
            var categories = _context.CaseReportForms
                                     .Include(f => f.CaseReportFormCategory)
                                     .GroupBy(f => f.CaseReportFormCategoryId)
                                     .ToList();

            var categoriesList = new List<SelectListItem>();
            foreach (var catGroup in categories)
            {
                var categoryId = catGroup.Key;
                var categoryModel = _context.CaseReportFormCategories
                                            .Where(c => c.ID == categoryId)
                                            .FirstOrDefault();
                var group = new SelectListGroup { Name = categoryModel.Name };

                foreach (var form in catGroup)
                {
                    var formListItem = new SelectListItem()
                    {
                        Value = form.ID.ToString(),
                        Text = form.Name,
                        Group = group
                    };
                    categoriesList.Add(formListItem);
                }
            }
            return categoriesList;
        }
    }
    }

