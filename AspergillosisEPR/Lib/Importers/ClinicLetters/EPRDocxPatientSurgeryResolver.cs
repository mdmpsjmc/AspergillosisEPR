using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using System.Text.RegularExpressions;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    public class EPRDocxPatientSurgeryResolver
    {
        private string _potentialSurgery;
        private Patient _patient;
        private AspergillosisContext _context;

        public EPRDocxPatientSurgeryResolver(string potentialSurgery, Patient patient, AspergillosisContext context)
        {
            _potentialSurgery = potentialSurgery;
            _patient = patient;
            _context = context;
            LoadSurgeriesForPatient();
        }

        internal PatientSurgery ResolveSurgery()
        {
            PatientSurgery patientSurgery = null;
            var matchingSurgery = FindMatchingSurgeriesInDatabase();
            PatientSurgery databaseSurgery = CheckIfMatchingSurgeryAlreadyExists(matchingSurgery);
            if (databaseSurgery == null && matchingSurgery != null)
            {
                patientSurgery = BuildSurgeryForPatient(matchingSurgery);
                _patient.PatientSurgeries.Add(patientSurgery);
                _context.PatientSurgeries.Add(patientSurgery);
            }
            else if (databaseSurgery != null && matchingSurgery != null)
            {               
                patientSurgery = databaseSurgery;
            }
            return patientSurgery;
        }

        private PatientSurgery BuildSurgeryForPatient(string matchingSurgery)
        {
            var patientSurgery = new PatientSurgery();
            var dbSurgery = _context.Surgeries.Where(s => s.Name.Equals(matchingSurgery))
                                              .FirstOrDefault();
            if (dbSurgery == null) return null;
            patientSurgery.Surgery = dbSurgery;
            patientSurgery.Note = _potentialSurgery;
            FindYearForSurgery(patientSurgery);
            return patientSurgery;
        }

        private void FindYearForSurgery(PatientSurgery patientSurgery)
        {
            Match matchesYear = RegularExpressions.ValidYear().Match(_potentialSurgery);
            if (matchesYear.Success)
            {
                patientSurgery.SurgeryDate = Int32.Parse(matchesYear.ToString());
            }
        }

        private PatientSurgery CheckIfMatchingSurgeryAlreadyExists(string matchingSurgery)
        {
            if (matchingSurgery == null) return null;
            if (_patient.PatientSurgeries == null) return null;
            foreach (var surgeryName in _patient.PatientSurgeries.Select(ps => ps.Surgery.Name))
            {
                if (surgeryName.ToLower().Equals(matchingSurgery.ToLower()))
                {
                    var patientSurgery = _patient.PatientSurgeries
                                                 .FirstOrDefault(pd => pd.Surgery.Name.ToLower().Equals(matchingSurgery.ToLower()));
                    return patientSurgery;
                }
            }
            return null;
        }

        private List<string> GetListOfSurgeriesFromDatabase()
        {
            return _context.Surgeries.Select(s => s.Name).ToList();
        }

        private string FindMatchingSurgeriesInDatabase()
        {
            var matched = new List<string>();
            foreach (var dbSurgery in GetListOfSurgeriesFromDatabase())
            {
                Regex regExpression = RegularExpressions.FindWordInList(dbSurgery.Split(" ").ToList());
                try
                {
                    if (regExpression.IsMatch(_potentialSurgery))
                    {
                        matched.Add(dbSurgery);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    continue;
                }
            }
            return matched.FirstOrDefault(el => _potentialSurgery.ToLower().Contains(el.ToLower()));
        }
        private void LoadSurgeriesForPatient()
        {
            _context.Entry(_patient).Collection(p => p.PatientSurgeries).Load();
            foreach(var surgery in _patient.PatientSurgeries)
            {
                _context.Entry(surgery).Reference(s => s.Surgery).Load();
            }
            if (_patient.PatientSurgeries == null)
            {
                _patient.PatientSurgeries = new List<PatientSurgery>();
            }
        }

    }
}
