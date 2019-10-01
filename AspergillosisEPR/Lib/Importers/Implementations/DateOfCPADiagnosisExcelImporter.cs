using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
  public class DateOfCPADiagnosisExcelImporter : SpreadsheetImporter
  {
    public DateOfCPADiagnosisExcelImporter(FileStream stream, IFormFile file,
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
        { "DiagnosisDate", "PatientNACDates.DateOfDiagnosis"}
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
      var allDates = _context.PatientNACDates
                             .Where(p => p.PatientId == patient.ID)
                             .FirstOrDefault();
      var dates = patient.PatientNACDates.FirstOrDefault();
      if (dates == null)
      {
        dates = new PatientNACDates();
      }
      dates.PatientId = patient.ID;
      for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
      {
        if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
        {
          ReadCell(patient, row, dates, cellCursor);
        }
      }

      patient.PatientNACDates.Add(dates);
      if (dates.PatientId != 0 && dates.PatientId > 0) Imported.Add(dates);
      return patient;
    }

    private void ReadCell(Patient patient, IRow row, PatientNACDates dates, int cellIndex)
    {
      string header = _headers.ElementAt(cellIndex);
      string newObjectFields = (string)_dictonary[header];
      string propertyValue = row.GetCell(cellIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK)
                                .ToString().Replace(" ", String.Empty);

      if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null)
      {
        var klassAndField = newObjectFields.Split(".");
        string propertyName = klassAndField[1];
        if (klassAndField[0] == "PatientNACDates")
        {
          if (propertyName == "DateOfDiagnosis")
          {
            try
            {
              var dateOfDiagnosis = DateTime.ParseExact(propertyValue, "dd/MM/yyyy", CultureInfo.InstalledUICulture);
              dates.DateOfDiagnosis = dateOfDiagnosis;
            }
            catch (Exception ex)
            {

              try
              {
                var yearDate = DateTime.ParseExact(propertyValue, "yyyy", CultureInfo.InstalledUICulture);
                dates.DateOfDiagnosis = yearDate;
              }
              catch (Exception ex2)
              {
                var yearDate = DateTime.ParseExact(propertyValue, "dd-MMM-yyyy", CultureInfo.InstalledUICulture);
                dates.DateOfDiagnosis = yearDate;
              }
            }

          }
        }
      }
    } 
  }
}
