using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Lib;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using System.Collections;
using System.Linq.Dynamic;
namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableRadiologyController : DataTablesController
    {

        public DataTableRadiologyController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load(string collection)
        {
            Action queriesAction = () =>
            {
                var results = QuerySideEffectsData(collection);
                foreach(var result in results)
                {
                    _list.Add(result);
                }
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }


        public IQueryable QuerySideEffectsData(string collection)
        {            
            switch (collection)
            {
                case "Finding":
                    return _aspergillosisContext.Query(typeof(Finding));
                case "ChestLocation":
                    return _aspergillosisContext.Query(typeof(ChestLocation));
                case "ChestDistribution":
                    return _aspergillosisContext.Query(typeof(ChestDistribution));
                case "Grade":
                    return _aspergillosisContext.Query(typeof(Grade));
                case "TreatmentResponse":
                    return _aspergillosisContext.Query(typeof(TreatmentResponse));
                case "RadiologyType":
                    return _aspergillosisContext.Query(typeof(RadiologyType));
            }
            return null;
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