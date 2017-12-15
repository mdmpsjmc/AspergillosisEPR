using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using NPOI.SS.Formula.Functions;
using AspergillosisEPR.Lib;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableDbImportsController : DataTablesController
    {
        private new List<DbImport> _list;

        public DataTableDbImportsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<DbImport>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            InitialSetup();

            _list = QueryDbImportsTable().ToList();
            Sorting();
            SingleSearch();

            _recordsTotal = _list.Count();

            var data = _list.Skip(_skip).
                                   Take(_pageSize).
                                   ToList();

            return JSONFromData(data);
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.ImportedFileName.Contains(_searchValue) || u.PatientsCount.ToString().Contains(_searchValue))
                        .ToList();
            }
        }

        private ICollection<DbImport> QueryDbImportsTable()
        {
            return (from import in _aspergillosisContext.DbImports select 
              new DbImport()
             {
                 ID = import.ID,
                 ImportedDate = import.ImportedDate,
                 ImportedFileName = import.ImportedFileName,
                 PatientsCount = import.PatientsCount
             }).ToList();
        }

        private void Sorting()
        {
            if (!(string.IsNullOrEmpty(_sortColumn) && string.IsNullOrEmpty(_sortColumnDirection)))
            {
                _list = _list.OrderBy(p => p.GetProperty(_sortColumn)).ToList();
                if (_sortColumnDirection == "desc")
                {
                    _list.Reverse();
                }
            }
        }
    }
}