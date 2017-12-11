using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class PatientDiagnosisResolver
    {
        private readonly Patient _patient;
        private readonly List<string> _potentialDiagnosisNames;
        private readonly AspergillosisContext _context;
        private List<PatientDiagnosis> _discoveredDiagnoses;
        private DiagnosisCategory _currentDiagnosisCategory;

        public PatientDiagnosisResolver(Patient patient, List<string> potentialDiagnosisNames, AspergillosisContext context)
        {
            _patient = patient;
            _potentialDiagnosisNames = potentialDiagnosisNames;
            _context = context;
            _discoveredDiagnoses = new List<PatientDiagnosis>(); 
        }

        public static List<string> PrimaryDiagnoses()
        {
            return new List<string>()
            {
                "CPA", "ABPA", "CCPA","SAFS"
            };
        }

        public List<PatientDiagnosis> Resolve()
        {
            foreach (string diagnosisName in _potentialDiagnosisNames)
            {
                if (PrimaryDiagnoses().Contains(diagnosisName))
                {
                    if (_discoveredDiagnoses.Count == 0)
                    {
                        _currentDiagnosisCategory = GetDiagnosisCategoryByName("Primary");
                        
                    } else if (_discoveredDiagnoses.Count == 1) 
                    {
                        _currentDiagnosisCategory = GetDiagnosisCategoryByName("Secondary");
                    }
                    DiagnosisType diagnosisType = GetDiagnosisByName(diagnosisName);
                    CreateAndAddPatientDiagnosis(diagnosisType);
                } else
                {
                    //var otherDiagnoses = _potentialDiagnosisNames.Except(PrimaryDiagnoses());
                    //if (otherDiagnoses == null)
                   // {
                   //     otherDiagnoses = _potentialDiagnosisNames;
                   // }
                   // foreach(string otherDiagnosis in otherDiagnoses)
                  //  {
                  //      _currentDiagnosisCategory = GetDiagnosisCategoryByName(SpreadsheetReader.OTHER_HEADER_NAME);
                  //      var diagnosisTypes = GetDiagnosisTypesFromOtherColumn(otherDiagnosis);
                  //      foreach(var diagType in diagnosisTypes)
                  //      {
                  //          CreateAndAddPatientDiagnosis(diagType);
                  //      }
                  //  }
                } 
            }
           
            return _discoveredDiagnoses;
        }

        private void CreateAndAddPatientDiagnosis(DiagnosisType diagnosisType)
        {         
            if (diagnosisType !=  null)         
            {
                var patientDiagnosis = new PatientDiagnosis();
                patientDiagnosis.DiagnosisTypeId = diagnosisType.ID;
                patientDiagnosis.PatientId = _patient.ID;
                patientDiagnosis.DiagnosisCategoryId = _currentDiagnosisCategory.ID;
                _discoveredDiagnoses.Add(patientDiagnosis);
                _patient.PatientDiagnoses = _discoveredDiagnoses;
            }            
        }

        private List<DiagnosisType> GetDiagnosisTypesFromOtherColumn(string diagnosisName)
        {
            string[] names = diagnosisName.Split(",");
            List<DiagnosisType> types = new List<DiagnosisType>();
            foreach(string name in names)
            {
                if (name == "") continue;
                string cleanedOutput;
                var output = Regex.Replace(name, @"[\d-]", string.Empty); // removes any digits
                if (name.Count(c => char.IsUpper(c)) > 1) {
                    cleanedOutput = string.Join("", name.Select(c => (char.IsUpper(c) ? c : '\0')).ToList()).Replace('\0'.ToString(), ""); //leaves only capital words for search
                } else
                {
                    cleanedOutput = output;
                }
                
                DiagnosisType diagnosisType = GetDiagnosisByName(cleanedOutput);
                if (diagnosisType != null)
                {
                    types.Add(diagnosisType);
                }
                else 
                {
                    diagnosisType = new DiagnosisType();
                    diagnosisType.Name = cleanedOutput;
                    types.Add(diagnosisType);
                    _context.SaveChanges();
                }          
            }
            return types;
        }

        private DiagnosisCategory GetDiagnosisCategoryByName(string categoryName)
        {
            return _context.DiagnosisCategories.Where(dc => dc.CategoryName.Equals(categoryName))
                                                .SingleOrDefault();
        }

        private DiagnosisType GetDiagnosisByName(string diagnosisName)
        {
            return _context.DiagnosisTypes.Where(dt => dt.ShortName.
                                           Contains(diagnosisName) || dt.Name.Contains(diagnosisName)).
                                           SingleOrDefault();
        }
    }
}
