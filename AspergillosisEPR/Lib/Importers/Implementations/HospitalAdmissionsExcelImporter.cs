using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
  public class HospitalAdmissionsExcelImporter : SpreadsheetImporter
  {
    public HospitalAdmissionsExcelImporter(FileStream stream, IFormFile file,
                                           string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

    {
    }
    public static Hashtable HeadersDictionary()
    {
      return new Hashtable()
      {
        { "SURNAME", "Patient.LastName" },
        { "FORENAME", "Patient.FirstName" },           
        { "SEX", "Patient.Gender"},
        { "Sex", "Patient.Gender"},
        { "DOB", "Patient.DOB"},              
        { "PreVisit", "PatientHospitalAdmission.PreVisit"},
        { "ICU", "PatientHospitalAdmission.ICU"},
        { "OneOrMore", "PatientHospitalAdmission.OneOrMoreAdmissions"},
        { "Multiple", "PatientHospitalAdmission.MoreThanOneAdmission"}
       };
    }

    protected override List<string> IdentiferHeaders()
    {
      return new List<string> { "RM2" };
    }

    protected override void ProcessSheet(ISheet currentSheet)
    {
      Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
      {
        ReadCellsForPatient(patient, row, cellCount);
        
      };
      InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
    }

    private Patient ReadCellsForPatient(Patient patient, IRow row, int cellCount)
    {
      var admission = new PatientHospitalAdmission();
      admission.PatientId = patient.ID;
      for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
      {
        if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
        {
          ReadCell(patient, row, admission, cellCursor);
        }
      }

      patient.PatientHospitalAdmissions.Add(admission);
      if (admission.PatientId != 0 && admission.PatientId > 0) Imported.Add(admission);
      return patient;
    }

    private void ReadCell(Patient patient, IRow row, PatientHospitalAdmission admission, int cellIndex)
    {
      string header = _headers.ElementAt(cellIndex);
      string newObjectFields = (string)_dictonary[header];
      string propertyValue = row.GetCell(cellIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();

      if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null)
      {
        var klassAndField = newObjectFields.Split(".");
        string propertyName = klassAndField[1];
        if (klassAndField[0] == "PatientHospitalAdmission")
        {
          if (propertyName == "PreVisit")
          {
            if (propertyValue == "1")
            {
              admission.PreVisit = true;
            }
          }
          if (propertyName == "ICU")
          {
            if (propertyValue == "1")
            {
              admission.ICU = true;
            }
          }
          if (propertyName == "OneOrMoreAdmissions")
          {
            if (propertyValue == "1")
            {
              admission.OneOrMoreAdmissions = true;
            }
          }
          if (propertyName == "MoreThanOneAdmission")
          {
            if (propertyValue == "1")
            {
              admission.MoreThanOneAdmission = true;
            }
          }
        }
      }
    }
  }
}
