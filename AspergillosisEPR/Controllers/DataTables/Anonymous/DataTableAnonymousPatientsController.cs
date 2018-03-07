using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.PatientViewModels.Anonymous;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers.DataTables.Anonymous
{
    public class DataTableAnonymousPatientsController : DataTablePatientsController
    {
        public DataTableAnonymousPatientsController(AspergillosisContext context) : base(context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        public IActionResult LoadPatients()
        {
            Action queriesAction = () =>
            {
                var patientData = QueryPatientData();

                var primaryDiagnosis = (from diagnosesCategories in _aspergillosisContext.DiagnosisCategories
                                        where diagnosesCategories.CategoryName == "Primary"
                                        select new { diagnosesCategories.ID }).Single();

                var patientIds = patientData.Select(pd => pd.ID).ToList();

                var patientDiagnoses = _aspergillosisContext.
                                        PatientDiagnoses.
                                        Include(pd => pd.DiagnosisType).
                                        Where(pd => patientIds.Contains(pd.PatientId) && pd.DiagnosisCategoryId == primaryDiagnosis.ID).
                                        ToList();

                _list = patientData.GroupBy(p => p.ID)
                                   .Select(a => a.FirstOrDefault())
                                   .ToList<dynamic>();
                AppendDiagnosesToPatients(patientDiagnoses);
                Search();
                Sorting();
            };
            return LoadData(queriesAction);
        }

        private void Search()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.ID.ToString().Contains(_searchValue) 
                                || u.Initials.Contains(_searchValue)).ToList();
            }
        }

        private IQueryable<AnonymousPatientDataTableViewModel> QueryPatientData()
        {
            return (from patient in _aspergillosisContext.Patients
                    join patientDiagnosis in _aspergillosisContext.PatientDiagnoses on patient.ID equals patientDiagnosis.PatientId into diagnoses
                    from patientDiagnosis in diagnoses.DefaultIfEmpty()
                    join diagnosesTypes in _aspergillosisContext.DiagnosisTypes on patientDiagnosis.DiagnosisTypeId equals diagnosesTypes.ID
                    into patientsWithDiagnoses
                    select new AnonymousPatientDataTableViewModel()
                    {
                        ID = patient.ID,
                        Initials = patient.Initials(),
                        PrimaryDiagnosis = ""
                    });
        }
    }
}