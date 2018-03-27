using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.CaseReportForms;
using AspergillosisEPR.Models.CaseReportForms.ViewModels;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableCaseReportBuiltFormsController : DataTablesController
    {

        public DataTableCaseReportBuiltFormsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                QueryCaseReportBuiltForms();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }


        public void QueryCaseReportBuiltForms()
        {
           var results =  _aspergillosisContext.CaseReportForms
                                        .Include(crf => crf.Sections)
                                            .ThenInclude(s => s.Section)
                                        .Include(crf => crf.CaseReportFormCategory)
                                        .Include(crf => crf.Fields)                                        
                                        .ToList<dynamic>();
            foreach (CaseReportForm result in results)
            {
                var viewModel = new CaseReportFormViewModel()
                {
                    ItemId = result.ID.ToString(),
                    IsLocked = result.IsLocked ? "<label class='label label-danger'>YES</label>" : "<label class='label label-success'>NO</label>",
                    Name = result.Name, 
                    CategoryName = result.CaseReportFormCategory.Name,
                    SectionsNames = result.Sections.Select(s => s.Section.Name).ToList(),
                    FieldsNames = result.Fields.Select(f => f.Label).ToList()
                };
                _list.Add(viewModel);
            }            
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(crf => crf.Name.ToString().Contains(_searchValue) 
                                    || crf.SectionsNames.Contains(_searchValue)).ToList();
            }
        }
    }
}