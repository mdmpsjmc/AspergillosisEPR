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
    public class DataTableCaseReportFormOptionGroupsController : DataTablesController
    {

        public DataTableCaseReportFormOptionGroupsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                QueryCaseReportOptionGroups();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }


        public void QueryCaseReportOptionGroups()
        {
           var results =  _aspergillosisContext.CaseReportFormOptionGroups
                                        .Include(crfog => crfog.Choices)
                                        .ToList<dynamic>();
            foreach(CaseReportFormOptionGroup result in results)
            {
                var viewModel  = new CaseReportFormOptionGroupViewModel();
                viewModel.ID = result.ID;
                viewModel.Name = result.Name;
                viewModel.Options = result.Choices.Select(c => c.Name).ToArray();
                _list.Add(viewModel);
            }            
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(crfog => crfog.Name.ToString().Contains(_searchValue) 
                                    || crfog.Options.Contains(_searchValue)).ToList();
            }
        }
    }
}