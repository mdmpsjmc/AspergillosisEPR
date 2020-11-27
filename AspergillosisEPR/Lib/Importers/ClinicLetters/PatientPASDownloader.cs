using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    class PatientPASDownloader
    {
        private List<string> _newRM2Numbers;
        private PASDbContext _pasContext;
        private AspergillosisContext _context;
        private int _patientAliveStatus;
        private int _patientDeceasedStatus;
        private DiagnosisType _cpaDiagnosis;
        private DiagnosisCategory _primaryDiagnosisCat;

        public PatientPASDownloader(List<string> rm2Numbers, AspergillosisContext context, PASDbContext pasContext)
        {
            _newRM2Numbers = rm2Numbers;            
            _context = context;
            _pasContext = pasContext;
            _patientAliveStatus = _context.PatientStatuses.Where(s => s.Name == "Active").FirstOrDefault().ID;
            _patientDeceasedStatus = _context.PatientStatuses.Where(s => s.Name == "Deceased").FirstOrDefault().ID;
            _cpaDiagnosis = _context.DiagnosisTypes.Where(dt => dt.Name.Contains("Chronic pulmonary aspergillosis")).SingleOrDefault();
            _primaryDiagnosisCat = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Primary").SingleOrDefault();
        }

        public List<Patient> AddMissingPatients()
        {
            var importedPatients = new List<Patient>();
            var lpiData = _pasContext.LpiPatientData
                        .Where(lpi => PrefixedWithRM2Numbers().Contains(lpi.FACIL_ID)
                                                    && !string.IsNullOrEmpty(lpi.FACIL_ID));

            foreach (var lpiPatient in lpiData)
            {
                var patient = GetPatient(lpiPatient.DistrictNumber());
                Console.WriteLine(lpiPatient.DistrictNumber());
                patient.FirstName = lpiPatient.FirstName();
                patient.LastName = lpiPatient.SURNAME;
                patient.DOB = lpiPatient.DateOfBirth();
                patient.Gender = lpiPatient.Gender();
                patient.NhsNumber = lpiPatient.NHS_NUMBER;
                patient.DistrictNumber = lpiPatient.DistrictNumber();
                patient.PatientStatusId = lpiPatient.PatientStatusId(_context, _patientDeceasedStatus, _patientAliveStatus);
                if (!string.IsNullOrEmpty(lpiPatient.DEATH_TIME)) patient.DateOfDeath = DateTime.ParseExact(lpiPatient.DEATH_TIME, "yyyyMMdd", CultureInfo.InvariantCulture);
                importedPatients.Add(patient);
                _context.Patients.Update(patient);
            }
            _context.SaveChanges();
            return importedPatients;
        }

        private Patient GetPatient(string rm2Number)
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

        public List<string> PrefixedWithRM2Numbers()
        {
            return _newRM2Numbers.Select(n => "RM2" + n).ToList();
        }
    }     
}
