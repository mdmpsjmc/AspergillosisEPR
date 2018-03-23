using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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

        public PatientDiagnosisResolver(Patient patient, AspergillosisContext context)
        {
            _patient = patient;
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

        public static List<string> ExisitingDiagnosisTypes(AspergillosisContext context)
        {
            var allDiagnoses = context.DiagnosisTypes;
            List<string> fullNames = allDiagnoses.Select(dt => dt.Name).
                                         ToList();
            List<string> shortNames = allDiagnoses.Where(dt => !string.IsNullOrEmpty(dt.ShortName)).
                                                   Select(dt => dt.ShortName).
                                                   ToList();
            var except = (IEnumerable<string>) PrimaryDiagnoses();
            return fullNames.Concat(shortNames).Except(except).ToList(); 
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
                        _currentDiagnosisCategory = GetDiagnosisCategoryByName("Other");                       
                    }
                    DiagnosisType diagnosisType = GetDiagnosisByName(diagnosisName);
                    CreateAndAddPatientDiagnosis(diagnosisType);
                }
                else
                {
                    var underlyingDiagnoses = _potentialDiagnosisNames.Except(PrimaryDiagnoses());
                    processUnderlyingDiagnoses(underlyingDiagnoses);
                } 
            }
           
            return _discoveredDiagnoses;
        }

        public List<PatientDiagnosis> ResolveForName(string diagnosisName, string categoryName)
        {
            _currentDiagnosisCategory = GetDiagnosisCategoryByName(categoryName);
            DiagnosisType diagnosisType = GetDiagnosisByName(diagnosisName);
            CreateAndAddPatientDiagnosis(diagnosisType);
            return _discoveredDiagnoses;
        }

        private void processUnderlyingDiagnoses(IEnumerable<string> underlyingDiagnoses)
        {
            if (underlyingDiagnoses == null)
            {
                underlyingDiagnoses = _potentialDiagnosisNames;
            }

            foreach (string underlyingDiagnosis in underlyingDiagnoses)
            {
                if (string.IsNullOrEmpty(underlyingDiagnosis)) continue;
                ProcessUnderlyingDiagnosis(underlyingDiagnosis);
            }
        }

        private void ProcessUnderlyingDiagnosis(string underlyingDiagnosis)
        {
            var existingDiagnoses = ExisitingDiagnosisTypes(_context);
            var words = GetDiagnosisTypesWords(underlyingDiagnosis);
            if (words.Count != 0)
            {               
                var matchingDiagnosesNames = existingDiagnoses.Where(ed => (SearchRegex(words).IsMatch(ed) && (ed.Split(" ").Count() < 4))).ToList();
                _currentDiagnosisCategory = GetDiagnosisCategoryByName("Underlying Diagnosis");
                foreach (var diagType in matchingDiagnosesNames)
                {
                    DiagnosisType dbDiagnosis = GetDiagnosisByName(diagType);
                    CreateAndAddPatientDiagnosis(dbDiagnosis, underlyingDiagnosis);
                }
            }
        }

        private Regex SearchRegex(List<string> words)
        {
            return new Regex(@"\b(" + string.Join("|", words.Select(Regex.Escape).ToArray()) + @"\b)", RegexOptions.IgnoreCase);
        }

        private void CreateAndAddPatientDiagnosis(DiagnosisType diagnosisType, string comment = null)
        {
            if (diagnosisType != null)
            {
                var patientDiagnosis = new PatientDiagnosis();
                patientDiagnosis.DiagnosisTypeId = diagnosisType.ID;
                patientDiagnosis.PatientId = _patient.ID;
                patientDiagnosis.DiagnosisCategoryId = _currentDiagnosisCategory.ID;
                if (!string.IsNullOrEmpty(comment))
                {
                    patientDiagnosis.Description = comment;
                }
                _discoveredDiagnoses.Add(patientDiagnosis);
                _patient.PatientDiagnoses = _discoveredDiagnoses;
            }            
        }

        private List<string> GetDiagnosisTypesWords(string diagnosisName)
        {

            List<string> words = new List<string>();
            if (diagnosisName.IndexOf(",") != -1) {
                string[] names = diagnosisName.Split(",");
                foreach(var name in names)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    words.AddRange(name.Trim().Split(null));
                }
            } else
            {
                var names = diagnosisName.Split(null);
                words.AddRange(words);
            }
            return words;
        }

        private DiagnosisCategory GetDiagnosisCategoryByName(string categoryName)
        {
            return _context.DiagnosisCategories.Where(dc => dc.CategoryName.Equals(categoryName))
                                                .SingleOrDefault();
        }

        private DiagnosisType GetDiagnosisByName(string diagnosisName)
        {
            return _context.DiagnosisTypes.Where(dt => dt.ShortName.
                                           Contains(diagnosisName) || dt.Name == diagnosisName).
                                           FirstOrDefault();
        }
    }
}
