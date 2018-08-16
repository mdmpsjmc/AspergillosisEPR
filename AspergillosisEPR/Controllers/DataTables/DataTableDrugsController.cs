using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
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
    public class DataTableDrugsController : DataTablesController
    {
        public DataTableDrugsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                var drugsData = QueryDrugsData().GroupBy(dt => dt.ID).Select(a => a.FirstOrDefault()).ToList();
                _list = drugsData.ToList<dynamic>();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        public IQueryable<Drug> QueryDrugsData()
        {
            return (from dt in _aspergillosisContext.Drugs
                    select new Drug()
                    {
                        ID = dt.ID,
                        Name = dt.Name
                    });
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(d => d.Name.ToLower().ToString().Contains(_searchValue.ToLower()))
                        .ToList();
            }
        }
    }
}
