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
    public class DataTableCaseReportFormsController : DataTablesController
    {
        private CaseReportFormsDataTableCollectionResolver _collectionResolver;

        public DataTableCaseReportFormsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load(string collection)
        {
            Action queriesAction = () =>
            {
                var results = QueryCaseReportFormsData(collection);
                foreach(var result in results)
                {
                    _list.Add(result);
                }
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }


        public IQueryable QueryCaseReportFormsData(string collection)
        {
            _collectionResolver = new CaseReportFormsDataTableCollectionResolver(_aspergillosisContext, collection);
            return _collectionResolver.Resolve();           
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