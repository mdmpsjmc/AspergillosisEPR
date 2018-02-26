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
                ColumnSearch();
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
                    VisitDate = DateHelper.DateTimeToUnixTimestamp(patientVisit.VisitDate),
                    PatientId = patientVisit.PatientId
                };
                visits.Add(patientRow);

            }
            return visits;
        }

        private void ColumnSearch()
        {
            for (int cursor = 0; cursor < 5; cursor++)
            {
                string partialSearch = Request.Form["columns[" + cursor.ToString() + "][search][value]"];
                if (partialSearch != null && partialSearch != "")
                {
                    switch (cursor)
                    {
                        case 1:
                            _list = _list.Where(p => DateTimeOffset.FromUnixTimeSeconds(long.Parse(p.VisitDate.ToString())).UtcDateTime.ToString().Contains(partialSearch)).ToList();
                            break;
                        case 2:
                            _list = _list.Where(p => p.PatientName.Contains(partialSearch)).ToList();
                            break;
                        case 3:
                            _list = _list.Where(p => p.RM2Number.Contains(partialSearch)).ToList();
                            break;
                        case 4:
                            _list = _list.Where(p => p.Examinations.Contains(partialSearch)).ToList();
                            break;                        
                    }
                }
            }
        }
    }
}
