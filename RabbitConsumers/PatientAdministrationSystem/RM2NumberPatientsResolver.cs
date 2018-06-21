using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using RabbitConsumers.DbFactories;
using RabbitConsumers.PatientAdministrationSystem.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RabbitConsumers.PatientAdministrationSystem
{
    class RM2NumberPatientsResolver
    {
        private List<string> _newRM2Numbers;
        private Data.PASDbContext _pasContext;
        private AspergillosisContext _context;
        private int _patientAliveStatus;
        private int _patientDeceasedStatus;
        private DiagnosisType _cpaDiagnosis;
        private DiagnosisCategory _primaryDiagnosisCat;

        public RM2NumberPatientsResolver()
        {
            var patientsList = new NewRM2PatientsList();
            _newRM2Numbers = patientsList.GetNewRM2Numbers();         
            _context = patientsList.Context();
            _patientAliveStatus = _context.PatientStatuses.Where(s => s.Name == "Active").FirstOrDefault().ID;
            _patientDeceasedStatus = _context.PatientStatuses.Where(s => s.Name == "Deceased").FirstOrDefault().ID;
            _cpaDiagnosis = _context.DiagnosisTypes.Where(dt => dt.Name.Contains("Chronic pulmonary aspergillosis")).SingleOrDefault();
            _primaryDiagnosisCat = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Primary").SingleOrDefault();
        }

        public List<Patient> Resolve()
        {
            using (_pasContext = new PASConnectionFactory().CreateDbContext())
            {
                var importedPatients = new List<Patient>();
                var lpiData = _pasContext.LpiPatientData                            
                            .Where(lpi => PrefixedWithRM2Numbers().Contains(lpi.FACIL_ID) 
                                                        && !string.IsNullOrEmpty(lpi.FACIL_ID));

                foreach (var lpiPatient in lpiData)
                {
                    var patient = GetPatient(lpiPatient.RM2Number());
                    Console.WriteLine("RM2 " + lpiPatient.RM2Number());
                    patient.FirstName = lpiPatient.FirstName();
                    patient.LastName = lpiPatient.SURNAME;
                    patient.DOB = lpiPatient.DateOfBirth();
                    patient.Gender = lpiPatient.Gender();
                    patient.NhsNumber = lpiPatient.NHS_NUMBER;
                    patient.RM2Number = lpiPatient.RM2Number();
                    patient.PatientStatusId = lpiPatient.PatientStatusId(_context, _patientDeceasedStatus, _patientAliveStatus);
                    if (!string.IsNullOrEmpty(lpiPatient.DEATH_TIME)) patient.DateOfDeath = DateTime.ParseExact(lpiPatient.DEATH_TIME, "yyyyMMdd", CultureInfo.InvariantCulture);
                    importedPatients.Add(patient);
                    UpdateTemporaryPatientAsImported(lpiPatient.RM2Number());
                    if (patient.ID == 0) AddPatientDiagnosis(patient);
                    _context.Patients.Update(patient);
                }
                _context.SaveChanges();
                return importedPatients;
            }
        }

        private  Patient GetPatient(string rm2Number)
        {
            Patient patient = null;
            patient = _context.Patients
                              .Where(p => p.RM2Number == rm2Number.Trim())
                              .FirstOrDefault();
            if (patient == null)
            {
                patient = new Patient();
            }
            return patient;
        }

        private void UpdateTemporaryPatientAsImported(string RM2Number)
        {
            var temporaryPatient = _context.TemporaryNewPatient
                                           .Where(p => p.RM2Number == RM2Number.Trim())
                                           .FirstOrDefault();
            if (temporaryPatient == null) return;
            temporaryPatient.ImportedAsRealPatient = true;
            _context.TemporaryNewPatient.Update(temporaryPatient);
        }

        public List<string> PrefixedWithRM2Numbers()
        {
            return _newRM2Numbers.Select(n => "RM2" + n).ToList();
        }

        private void AddPatientDiagnosis(Patient patient)
        {
            var patientDiagnosis = new PatientDiagnosis();
            patient.PatientDiagnoses = new List<PatientDiagnosis>();
            patientDiagnosis.DiagnosisCategoryId = _primaryDiagnosisCat.ID;
            patientDiagnosis.DiagnosisTypeId = _cpaDiagnosis.ID;
            patientDiagnosis.PatientId = patient.ID;
            patient.PatientDiagnoses.Add(patientDiagnosis);
        }



    }
}