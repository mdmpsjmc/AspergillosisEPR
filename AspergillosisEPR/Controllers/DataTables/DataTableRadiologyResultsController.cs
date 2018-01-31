using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Controllers.DataTables;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.DataTableViewModels;

namespace AspergillosisEPR.Controllers
{
    public class DataTableRadiologyResultsController : DataTablesController
    {

        public DataTableRadiologyResultsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                var radiologyResults = QueryRadiologyResultsData().ToList();
                var allOptions = _aspergillosisContext.RadiologyFindingSelectOption.ToList();
                //AppendMissingDataInRadiologyResults(allOptions);
                _list = radiologyResults.ToList<dynamic>();
                //Sorting();                
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        public IQueryable<RadiologyResultDataTableViewModel> QueryRadiologyResultsData()
        {
            return (from rresult in _aspergillosisContext.RadiologyResult
                    join radiologyFindingSelect in _aspergillosisContext.RadiologyFindingSelect on rresult.RadiologyFindingSelectId equals radiologyFindingSelect.ID into radiologyFindingSelects
                    from radiologyFindingSelect in radiologyFindingSelects.DefaultIfEmpty()
                    join radiologyFindingSelectOption in _aspergillosisContext.RadiologyFindingSelectOption on rresult.RadiologyFindingSelectOptionId equals radiologyFindingSelectOption.ID
                    into radiologyResultsWithRelatedData
                    select new RadiologyResultDataTableViewModel()
                    {
                        ID = rresult.ID,
                        Name = rresult.RadiologyFindingSelect.Name,
                        SelectId = rresult.RadiologyFindingSelectId,
                        Option = rresult.RadiologyFindingSelectOption.Name,
                        IsMulti = rresult.IsMultiple ? "YES" : "NO"
                    });
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
            }
        }

    }
}