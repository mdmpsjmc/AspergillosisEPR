using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models.DataTableViewModels;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableMedicalTrialsController : DataTablesController
    {
        public DataTableMedicalTrialsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {

            Action queriesAction = () => {
                QueryTrialsTable();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        private void QueryTrialsTable()
        {
            var medicalTrials = _aspergillosisContext.MedicalTrials
                                                     .OrderBy(t => t.ID)
                                                     .ToList();
            foreach(var medicalTrial in medicalTrials)
            {
                var trialVm = new MedicalTrialDataTableViewModel()
                {
                    ID = medicalTrial.ID,
                    Name = medicalTrial.Name,
                    Description = medicalTrial.Description
                };
                _list.Add(trialVm);
            }
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.Name.Contains(_searchValue)
                                || u.Description.Contains(_searchValue))
                        .ToList();
            }
        }
    }
}