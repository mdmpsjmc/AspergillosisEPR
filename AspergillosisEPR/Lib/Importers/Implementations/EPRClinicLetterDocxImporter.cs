using AspergillosisEPR.Data;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using NPOI.XWPF.Extractor;
using DocumentFormat.OpenXml;
using AspergillosisEPR.Lib.Importers.ClinicLetters;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class EPRClinicLetterDocxImporter : Importer
    {
        WordprocessingDocument _wordDocument;
        PatientManager _patientManager;
        private static string DIAGNOSES_IDENTIFIER = "Diagnoses:";
        private List<PatientDiagnosis> _diagnoses;
        private List<PatientSurgery> _surgeries;
        private Package _wordPackage;
        private MsWordFormatConverter _wordConverter;

        public EPRClinicLetterDocxImporter(FileStream stream, 
                                              IFormFile file,
                                              string fileExtension, AspergillosisContext context)
        {
            _stream = stream;
            _file = file;     
            _fileExtension = fileExtension;
            Imported = new List<dynamic>();
            _context = context;
            _patientManager = new PatientManager(context);
            ReadDOCXFile();
            
        }

        private void ReadDOCXFile()
        {
            ResetStream();
            ConvertToDocxIfDoc();
            InitializeRequiredProperties();
            using (_wordDocument = WordprocessingDocument.Open(_wordPackage))
            {
                var diagnosisElement = GetDiagnosisElement();
                var rm2Number = FindRM2InDocumentName();
                var potentialDiagnoses = FindDiagnosesFromWord();
                List<string> discoveredDiagnosesList = CopyToNewList(potentialDiagnoses);
                Patient patient = _patientManager.FindPatientByRM2Number(rm2Number, true);
                if (patient == null) return;
                foreach (var potentialDiagnosis in potentialDiagnoses)
                {
                    var diagnosis = GetPatientDiagnosesFromWord(patient, potentialDiagnosis);
                    if (diagnosis != null)
                    {
                        Imported.Add(diagnosis);
                        discoveredDiagnosesList.Remove(potentialDiagnosis);
                    }
                    GetPatientSurgeriesFromWord(patient, potentialDiagnosis);
                }
                BuildPatientGenericNotes(patient, discoveredDiagnosesList);
            }
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
            string listAsString = string.Join(", ", discoveredDiagnosesList);
            if (string.IsNullOrEmpty(patient.GenericNote))
            {
                patient.GenericNote = listAsString;
            } else
            {
                patient.GenericNote = patient.GenericNote + ", " + listAsString;
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
            var rm2 = RegularExpressions.JustDigits().Match(_file.Name);
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
                } else
                {
                    diagnosesFromWord.Add(element.InnerText.Replace(DIAGNOSES_IDENTIFIER, String.Empty).Trim());
                }              
            }
            return diagnosesFromWord; 
        }

        private OpenXmlElement GetDiagnosisElement()
        {
            return MainDocumentParagraphs()
                               .First(e => e.InnerText.Contains(DIAGNOSES_IDENTIFIER));
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
        private void ResetStream()
        {
            _file.CopyTo(_stream);
            _stream.Position = 0;
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
    }
}