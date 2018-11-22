using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Models.DataTableViewModels;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspergillosisEPR.Controllers.DataTables
{
    [Authorize]
    public class DataTableICD10DiagnosesController : DataTablesController
    {
        public DataTableICD10DiagnosesController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load(string patientId)
        {
            Action queriesAction = () =>
            {
                var icd10DiagnosesData = QueryICD10Data(patientId).GroupBy(dt => dt.ID).Select(a => a.FirstOrDefault()).ToList();
                _list = icd10DiagnosesData.ToList<dynamic>();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        public List<PatientICD10DiagnosisDataTableViewModel> QueryICD10Data(string patientId)
        {
            var dxList = _aspergillosisContext.PatientICD10Diagnoses
                                                    .Where(diag => diag.PatientId.Equals(Int32.Parse(patientId)))                       
                                                    .ToList();

            var diagnoses = new List<PatientICD10DiagnosisDataTableViewModel>();
            foreach(var dx in dxList)
            {
                var viewModel = new PatientICD10DiagnosisDataTableViewModel();
                viewModel.ID = dx.ID;
                viewModel.DiagnosisCode = dx.DiagnosisCode;
                viewModel.DiagnosisDate = DateHelper.DateTimeToUnixTimestamp(dx.DiagnosisDate);
                viewModel.DiagnosisDescription = dx.DiagnosisDescription;
                diagnoses.Add(viewModel);
            }
            return diagnoses;                        
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(d => d.DiagnosisDescription.ToLower().ToString().Contains(_searchValue.ToLower()))
                        .ToList();
            }
        }
    }
}