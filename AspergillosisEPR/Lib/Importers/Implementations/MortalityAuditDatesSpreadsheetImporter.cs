using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
  public class MortalityAuditDatesSpreadsheetImporter : SpreadsheetImporter
    {
  
    public static string[] IdentifierHeaders = { "RM2Number" };
    public MortalityAuditDatesSpreadsheetImporter(FileStream stream, IFormFile file,
                             string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

    {
    }

    public static Hashtable HeadersDictionary()
    {
      return new Hashtable()
            {
                  { "FirstName", "Patient.LastName" },
                  { "LastName", "Patient.LastName"},
                  { "RM2Number", "Patient.RM2Number" },
                  { "ReferralDate", "PatientNACDates.ReferralDate"},
                  { "FirstSeenAtNAC", "PatientNACDates.FirstSeenAtNAC"},
                  { "LastObservationPoint", "PatientNACDates.LastObservationPoint"},
                  { "DateOfDeath", "PatientNACDates.DateOfDeath"}
             };
    }

    protected override void ProcessSheet(ISheet currentSheet)
    {
      Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
      {
       
        patient = ReadRowCellsIntoPatientObject(patient, row, cellCount);
        var existingPatient = ExistingPatient(patient.RM2Number);
        Imported.Add(patient);
      };
      InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
    }

    private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount)
    {
      List<PatientNACDates> dates = _context.PatientNACDates.Where(d => d.PatientId.Equals(patient.ID)).ToList();
      for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
      {
        if (row.GetCell(cellCursor) != null)
        {
          ReadCell(patient, row, cellCursor, dates);
        }
      }

      return patient;
    }

    private void ReadCell(Patient patient, IRow row, int cellIndex, List<PatientNACDates> dates)
    {

      string header = _headers.ElementAt(cellIndex);
      string propertyValue = row.GetCell(cellIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
      if (dates.Count == 0)
      {
        var nacDate = new PatientNACDates() { Patient = patient };
        patient.PatientNACDates.Add(nacDate);
        dates = patient.PatientNACDates.ToList();
      }

      string newObjectFields = (string)_dictonary[header];
      if (newObjectFields != null)
      {
        string[] fields = newObjectFields.Split("|");
        foreach (string field in fields)
        {
          var klassAndField = field.Split(".");
          switch (klassAndField[0])
          {
            case "Patient":

              break;
            case "PatientNACDates":
              try
              {
                var date = dates.FirstOrDefault();
                string propertyName = klassAndField[1];


                Type type = date.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?) && !string.IsNullOrEmpty(propertyValue))
                {
                  propertyInfo.
                       SetValue(date, Convert.ChangeType(propertyValue, typeof(DateTime)), null);
                }
              } catch(Exception e)
              {
                Console.WriteLine(e.Message);
                Console.WriteLine(propertyValue);
                Console.WriteLine(klassAndField[1]);
              }
                
             break;
          }
        }
      }
    }

    protected override List<string> IdentiferHeaders()
    {
      return IdentifierHeaders.ToList();
    }
  }
}
