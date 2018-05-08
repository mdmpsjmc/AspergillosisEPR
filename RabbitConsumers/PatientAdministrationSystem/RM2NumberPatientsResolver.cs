using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
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
        private PASDbContext _pasContext;
        private AspergillosisContext _context;
        private int _patientAliveStatus;
        private int _patientDeceasedStatus;

        public RM2NumberPatientsResolver()
        {
            _newRM2Numbers = new NewRM2PatientsList().GetNewRM2Numbers();
            _pasContext = new PASConnectionFactory().CreateDbContext();
            _context = new AspergillosisContextFactory().CreateDbContext();
            _patientAliveStatus = _context.PatientStatuses.Where(s => s.Name == "Active").FirstOrDefault().ID;
            _patientDeceasedStatus = _context.PatientStatuses.Where(s => s.Name == "Deceased").FirstOrDefault().ID;
        }

        public List<Patient> Resolve()
        {
           var importedPatients = new List<Patient>();
           var lpiData =  _pasContext.LpiPatientData
                       .Where(lpi => PrefixedWithRM2Numbers().Contains(lpi.FACIL_ID));

            foreach(var lpiPatient in lpiData)
            {
                var patient = new Patient()
                {
                    FirstName = lpiPatient.FirstName(),
                    LastName = lpiPatient.SURNAME,
                    DOB = lpiPatient.DOB,
                    Gender = lpiPatient.Gender(),
                    NhsNumber = lpiPatient.NHS_NUMBER,
                    RM2Number = lpiPatient.RM2Number(),
                    PatientStatusId = lpiPatient.PatientStatusId(_context, _patientDeceasedStatus, _patientAliveStatus),
                    DateOfDeath = DateTime.ParseExact(lpiPatient.DEATH_TIME, "yyyyMMdd", CultureInfo.InvariantCulture)
                };
                importedPatients.Add(patient);
            }
            return importedPatients;
        }

        public List<string> PrefixedWithRM2Numbers()
        {
            return _newRM2Numbers.Select(n => "RM2" + n).ToList();
        }
    }
}
