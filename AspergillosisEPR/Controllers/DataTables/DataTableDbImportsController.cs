using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models.DataTableViewModels;
using AspergillosisEPR.Lib;
using System;
using AspergillosisEPR.Helpers;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableDbImportsController : DataTablesController
    {

        public DataTableDbImportsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            
            Action queriesAction = () => {
                _list = QueryDbImportsTable().ToList<dynamic>();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);            
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.ImportedFileName.Contains(_searchValue)
                                || u.PatientsCount.ToString().Contains(_searchValue))
                        .ToList();
            }
        }

        private ICollection<DbImportsTableViewModel> QueryDbImportsTable()
        {
            return (from import in _aspergillosisContext.DbImports
                    orderby import.ImportedDate descending
                    join importType in _aspergillosisContext.DBImportTypes on import.DbImportTypeId equals importType.ID
                    select
              new DbImportsTableViewModel()
             {
                 ID = import.ID,
                 ImportedDate = DateHelper.DateTimeToUnixTimestamp(import.ImportedDate),
                 ImportedFileName = import.ImportedFileName,
                 ImportTypeName = importType.Name,
                 PatientsCount = import.PatientsCount
             }).ToList();
        }
    }
}