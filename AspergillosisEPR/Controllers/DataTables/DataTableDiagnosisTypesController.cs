using AspergillosisEPR.Data;
using AspergillosisEPR.Models.DataTableViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Controllers.DataTables
{
    [Authorize]
    public class DataTableDiagnosisTypesController : DataTablesController
    {
        public DataTableDiagnosisTypesController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                var diagnosesTypesData = QueryDiagnosesTypesData().GroupBy(dt => dt.ID).Select(a => a.FirstOrDefault()).ToList();
                _list = diagnosesTypesData.ToList<dynamic>();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        public IQueryable<DiagnosisTypeDataTableViewModel> QueryDiagnosesTypesData()
        {
            return (from dt in _aspergillosisContext.DiagnosisTypes
                    select new DiagnosisTypeDataTableViewModel()
                    {
                        ID = dt.ID,
                        Name = dt.Name,
                        ShortName = dt.ShortName,                       
                    });
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => (u.ShortName == null ? "".Contains(_searchValue) : u.ShortName.Contains(_searchValue))
                                       || u.Name.ToString().Contains(_searchValue))
                        .ToList();
            }
        }
    }
}
