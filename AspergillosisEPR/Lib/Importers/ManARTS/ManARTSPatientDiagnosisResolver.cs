using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using AspergillosisEPR.Data;
using NPOI.SS.UserModel;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public class ManARTSPatientDiagnosisResolver
    {
        private Patient _patient;
        private List<string> _diagnosisNames;
        private AspergillosisContext _context;
        private DiagnosisType _dbDiagnosis;
        private ManARTSDiagnosis _diagnosisFromExcel;

        public ManARTSPatientDiagnosisResolver(Patient patient,
                                                string diagnosisName,
                                                AspergillosisContext context,
                                                ManARTSDiagnosis diagnosisFromExcel)
        {
            _patient = patient;
            _context = context;
            _diagnosisNames = new List<string> { diagnosisName };
            _dbDiagnosis = FindDiagnosisByName(diagnosisName);
            _diagnosisFromExcel = diagnosisFromExcel;
        }     

        public List<PatientDiagnosis> Resolve()
        {
            var patientDiagnoses = new List<PatientDiagnosis>();
            IEnumerable<string> existingDiagnosesNames = GetExistingDiagnosesNames();
            var diagnosisToAdd = existingDiagnosesNames.Except(_diagnosisNames).ToList();
            if (diagnosisToAdd.Count == 0)
            {
                return patientDiagnoses;
            } else
            {
                patientDiagnoses.Add(BuildDiagnosis());
            }
            return patientDiagnoses;
        }

        private PatientDiagnosis BuildDiagnosis()
        {
            CreateDiagnosisTypeIfNotExist();
            var patientDiagnosis = new PatientDiagnosis();
            patientDiagnosis.Patient = _patient;
            patientDiagnosis.DiagnosisType = _dbDiagnosis;
            patientDiagnosis.Description = _diagnosisFromExcel.Notes.Trim();            
            return patientDiagnosis;
        }

        private void CreateDiagnosisTypeIfNotExist()
        {
            if (_dbDiagnosis == null)
            {
                _dbDiagnosis = new DiagnosisType()
                {
                    Name = _diagnosisNames[0]
                };
                _context.DiagnosisTypes.Add(_dbDiagnosis);
                _context.SaveChanges();
            }
        }

        private IEnumerable<string> GetExistingDiagnosesNames()
        {
            return _patient.PatientDiagnoses.Select(pd =>
            {
                if (string.IsNullOrEmpty(pd.DiagnosisType.ShortName))
                {
                    return pd.DiagnosisType.Name;
                }
                else
                {
                    return pd.DiagnosisType.ShortName;
                }
            });
        }

        private DiagnosisType FindDiagnosisByName(string diagnosisName)
        {
            return _context.DiagnosisTypes
                                   .Where(dt => dt.Name == diagnosisName || dt.ShortName == diagnosisName)
                                   .FirstOrDefault();
        }
    }
}
