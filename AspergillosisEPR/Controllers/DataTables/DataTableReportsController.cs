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
using AspergillosisEPR.Models.DataTableViewModels;
using AspergillosisEPR.Models.Reporting;
using AspergillosisEPR.Helpers;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableReportsController : DataTablesController
    {
        public DataTableReportsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role, Reporting Role")]
        public IActionResult Load(string collection)
        {
            Action queriesAction = () =>
            {
                var results = QueryReportsData();
                foreach(var result in results)
                {
                    var reportModel = (Report)result;
                    var reportVM = new ReportDataTableVM();
                    reportVM.ID = reportModel.ID;
                    reportVM.ReportType = reportModel.ReportType.Name;
                    reportVM.StartDate = DateHelper.DateTimeToUnixTimestamp(reportModel.StartDate);
                    reportVM.EndDate = DateHelper.DateTimeToUnixTimestamp(reportModel.EndDate);
                    reportVM.Patients = reportModel.PatientReportItems.Select(p => p.PatientId).Distinct().Count();
                    _list.Add(reportVM);
                }
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }


        public IQueryable QueryReportsData()
        {
            return _aspergillosisContext.Reports
                                        .Include(r => r.PatientReportItems)
                                        .Include(r => r.ReportType);            
        }
        
        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.ReportType.ToLower().Contains(_searchValue.ToLower())).ToList();
            }
        }

    }
}