using AspergillosisEPR.Controllers.DataTables;
using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Controllers
{
    public class DataTablePatientVisitsController : DataTablesController
    {
        public DataTablePatientVisitsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                _list = QueryVisitsData().ToList<dynamic>();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        public List<PatientVisitsDataTableViewModel> QueryVisitsData()
        {
            var patientsWithVisits = (from patientVisit in _aspergillosisContext.PatientVisits
                                      join patientExaminations in _aspergillosisContext.PatientExaminations on patientVisit.ID equals patientExaminations.PatientVisitId into examinations
                                      select patientVisit
                                      );

            var visits = new List<PatientVisitsDataTableViewModel>();
            foreach(var patientVisit in patientsWithVisits)
            {
                var patient = _aspergillosisContext.Patients.Where(p => p.ID == patientVisit.PatientId).SingleOrDefault();
                patientVisit.Patient = patient;
                var patientRow = new PatientVisitsDataTableViewModel()
                {
                    ID = patientVisit.ID,
                    Examinations = _aspergillosisContext.PatientExaminations.Where(pe => pe.PatientVisitId == patientVisit.ID).Select(pe => PatientVisitsDataTableViewModel.ExaminationNameFromClass(pe.Discriminator)).ToList(),
                    PatientName = patientVisit?.Patient.FullName,
                    RM2Number = patientVisit?.Patient.RM2Number,
                    VisitDate = DateHelper.DateTimeToUnixTimestamp(patientVisit.VisitDate)
                };
                visits.Add(patientRow);

            }
            return visits;
        }
            

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                    .Where(u => u.Examinations.Contains(_searchValue) || u.PatientName.ToString().Contains(_searchValue)
                                || u.RM2Number.ToString().Contains(_searchValue)
                                || DateTimeOffset.FromUnixTimeSeconds(long.Parse(u.VisitDate)).UtcDate.ToString().Contains(_searchValue)).
                                ToList();                
            }
        }
    }
}
