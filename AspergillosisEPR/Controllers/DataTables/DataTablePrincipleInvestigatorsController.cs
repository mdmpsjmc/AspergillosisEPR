using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.DataTableViewModels;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTablePrincipleInvestigatorsController : DataTablesController
    {
        public DataTablePrincipleInvestigatorsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {

            Action queriesAction = () => {
                QueryInvestigatorsTable();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        private void QueryInvestigatorsTable()
        {
            var investigators = _aspergillosisContext.MedicalTrialsPrincipalInvestigators
                                                     .Include(i => i.PersonTitle)
                                                     .ToList();
            foreach(var investigator in investigators)
            {
                var investigatorVM = new PrincipalInvestigatorDataTableViewModel()
                {
                    ID = investigator.ID,
                    FirstName = investigator.FirstName,
                    LastName = investigator.LastName,
                    Title = investigator.PersonTitle.Name
                };
                _list.Add(investigatorVM);
            }                                 
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.FirstName.Contains(_searchValue)
                                || u.LastName.Contains(_searchValue))
                        .ToList();
            }
        }
    }
}