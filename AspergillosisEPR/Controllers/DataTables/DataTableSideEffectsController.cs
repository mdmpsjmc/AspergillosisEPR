using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableSideEffectsController : DataTablesController
    {

        public DataTableSideEffectsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                var sideEffects = QuerySideEffectsData().GroupBy(dt => dt.ID).
                                                         Select(a => a.FirstOrDefault()).
                                                         ToList();
                _list = sideEffects.ToList<dynamic>();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        public IQueryable<SideEffect> QuerySideEffectsData()
        {
            return (from dt in _aspergillosisContext.SideEffects
                    select new SideEffect()
                    {
                        ID = dt.ID,
                        Name = dt.Name,
                    });
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.Name.ToString().Contains(_searchValue)).ToList();
            }
        }
    }
       
}