using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    public class EPRClinicLetterDocxImporter : Importer
    {
        private static string DIAGNOSES_IDENTIFIER = "Diagnoses:";
        private static string ALTERNATE_DIAGNOSES_IDENTIFIER = "Diagnosis:";
        WordprocessingDocument _wordDocument;
        private PASDbContext _pasContext;
        PatientManager _patientManager;
        private List<PatientDiagnosis> _diagnoses;
        private List<PatientSurgery> _surgeries;
        private Package _wordPackage;
        private MsWordFormatConverter _wordConverter;
        private bool _skipStreamReset = false;

        public EPRClinicLetterDocxImporter(FileStream stream, string fileExtension, 
                                           AspergillosisContext context, PASDbContext pasContext)
        {
            _stream = stream;
            _fileExtension = fileExtension;
            _pasContext = pasContext;
            _context = context;
            _patientManager = new PatientManager(context);
        }

        public void ReadDOCXFile()
        {
            ConvertToDocxIfDoc();
            InitializeRequiredProperties();
            using (_wordDocument = WordprocessingDocument.Open(_wordPackage))
            {
                var diagnosisElement = GetDiagnosisElement();
                if (diagnosisElement == null) return;
                var rm2Number = FindRM2InDocumentName();
                var potentialDiagnoses = FindDiagnosesFromWord();
                List<string> discoveredDiagnosesList = CopyToNewList(potentialDiagnoses);
                Patient patient = _patientManager.FindPatientByRM2Number(rm2Number, true);

                if (patient == null) patient = DownloadPatientFromPAS(rm2Number);
                if (patient == null) return;

                foreach (var potentialDiagnosis in potentialDiagnoses)
                {
                    var diagnosis = GetPatientDiagnosesFromWord(patient, potentialDiagnosis);
                    if (diagnosis != null)
                    {
                        discoveredDiagnosesList.Remove(potentialDiagnosis);
                    }
                    GetPatientSurgeriesFromWord(patient, potentialDiagnosis);
                }
                BuildPatientGenericNotes(patient, discoveredDiagnosesList);
            }
            _stream.Close();
            _context.SaveChanges();
        }

        private PatientSurgery GetPatientSurgeriesFromWord(Patient patient, string potentialSurgery)
        {
            var dataResolver = new EPRDocxPatientSurgeryResolver(potentialSurgery, patient, _context);
            PatientSurgery surgery = dataResolver.ResolveSurgery();
            if (surgery != null) _surgeries.Add(surgery);
            return surgery;
        }

        private void BuildPatientGenericNotes(Patient patient, List<string> discoveredDiagnosesList)
        {
            foreach(var note in discoveredDiagnosesList)
            {
                if (string.IsNullOrEmpty(patient.GenericNote))
                {
                    patient.GenericNote = note;
                }
                else
                {
                    if (!patient.GenericNote.Contains(note))
                    {
                        patient.GenericNote = patient.GenericNote + ", " + note;
                    }                   
                }
            }            
        }

        private static List<string> CopyToNewList(List<string> potentialDiagnoses)
        {
            var discoveredDiagnoses = new String[potentialDiagnoses.Count];
            potentialDiagnoses.CopyTo(discoveredDiagnoses);
            var discoveredDiagnosesList = discoveredDiagnoses.ToList();
            return discoveredDiagnosesList;
        }

        private PatientDiagnosis GetPatientDiagnosesFromWord(Patient patient, string potentialDiagnosis)
        {
            var dataResolver = new EPRDocxPatientDiagnosisResolver(potentialDiagnosis, patient, _context);
            PatientDiagnosis diagnosis = dataResolver.ResolveDiagnosis();
            if (diagnosis != null) _diagnoses.Add(diagnosis);
            return diagnosis;
        }

        private string FindRM2InDocumentName()
        {
            string fileName = Path.GetFileName(_stream.Name);
            var rm2 = RegularExpressions.JustDigits().Match(fileName);
            return rm2.ToString();
        }

        private List<string> FindDiagnosesFromWord()
        {
            var diagnosesFromWord = new List<string>();
            var elementsSublist = MainDocumentParagraphs()
                                           .Where(z => MainDocumentParagraphs().IndexOf(z) >= GetDiagnosisElementIndex());

            foreach (var element in elementsSublist)
            {
                if (string.IsNullOrEmpty(element.InnerText))
                {
                    break;
                }
                else
                {
                    diagnosesFromWord.Add(element.InnerText.Replace(DIAGNOSES_IDENTIFIER, String.Empty).Trim());
                }
            }
            return diagnosesFromWord;
        }

        private OpenXmlElement GetDiagnosisElement()
        {
            OpenXmlElement diagnosisElement = MainDocumentParagraphs()
                                             .FirstOrDefault(e => e.InnerText.Contains(DIAGNOSES_IDENTIFIER));
            if (diagnosisElement == null)
            {
                return MainDocumentParagraphs()
                                   .FirstOrDefault(e => e.InnerText.Contains(ALTERNATE_DIAGNOSES_IDENTIFIER));
            }
            else
            {
                return diagnosisElement;
            }
        }

        private int GetDiagnosisElementIndex()
        {
            return MainDocumentParagraphs().IndexOf(GetDiagnosisElement());
        }

        private List<OpenXmlElement> MainDocumentParagraphs()
        {
            return _wordDocument.MainDocumentPart
                                .Document
                                .Body
                                .ChildElements
                                .ToList();
        }

        private void InitializeRequiredProperties()
        {
            _wordPackage = Package.Open(_stream, FileMode.Open);
            _diagnoses = new List<PatientDiagnosis>();
            _surgeries = new List<PatientSurgery>();
        }


        private void ConvertToDocxIfDoc()
        {
            if (_fileExtension.Equals(".doc"))
            {
                _stream.Close();
                MsWordFormatConverter _wordConverter = new MsWordFormatConverter(_stream.Name, true);
                string filePath = _wordConverter.ConvertDocToDocx();
                _stream = File.Open(filePath, FileMode.Open);
            }
        }
        private Patient DownloadPatientFromPAS(string rm2Number)
        {
            Patient patient;
            PatientPASDownloader downloader = new PatientPASDownloader(new List<string>() { rm2Number }, _context, _pasContext);
            var patients = downloader.AddMissingPatients();
            if (patients.Count == 0) return null;
            patient = patients[0];
            return patient;
        }
    }
}
