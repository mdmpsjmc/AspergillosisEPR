using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
  public class MortalityAuditWeightAndHeightImporter : SpreadsheetImporter
    {
  
    public static string[] IdentifierHeaders = { "RM2Number" };
    public MortalityAuditWeightAndHeightImporter(FileStream stream, IFormFile file,
                             string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

    {
    }

    public static Hashtable HeadersDictionary()
    {
      return new Hashtable()
      {
                  { "RM2Number", "Patient.RM2Number" },
                  { "HeightInCm", "PatientMeasurement.Height"},
                  { "Weight", "PatientMeasurement.Weight"},
                  { "DateTaken", "PatientMeasurement.DateTaken"},
      };
    }

    protected override void ProcessSheet(ISheet currentSheet)
    {
      Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
      {
        List<PatientMeasurement> mrcs = _context.PatientMeasurements.Where(d => d.PatientId.Equals(patient.ID)).ToList();
        var newWeightHeight = new PatientMeasurement() { PatientId = patient.ID };
        patient = ReadRowCellsIntoPatientObject(patient, row, cellCount, newWeightHeight);
        var existingDates = mrcs.Select(pi => pi.DateTaken.Date).ToList();        
        bool dateDoesNotExist = existingDates.FindAll(d => d.Date == newWeightHeight.DateTaken.Date).ToList().Count == 0;
        if (newWeightHeight.DateTaken.Year > 1 && dateDoesNotExist)
        {

          if (patient.PatientMeasurements == null) patient.PatientMeasurements = new List<PatientMeasurement>();
          patient.PatientMeasurements.Add(newWeightHeight);

        }
        Imported.Add(patient);
      };
      InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
    }

    private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount, PatientMeasurement newWeightHeight)
    {
      for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
      {
        if (row.GetCell(cellCursor) != null)
        {
         
          ReadCell(patient, row, cellCursor, newWeightHeight);      
        }
      }

      return patient;
    }

    private void ReadCell(Patient patient, IRow row, int cellIndex, PatientMeasurement newWeightHeight)
    {
      string header = _headers.ElementAt(cellIndex);
      string propertyValue = row.GetCell(cellIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
   

      string newObjectFields = (string)_dictonary[header];
      if (newObjectFields != null)
      {
        string[] fields = newObjectFields.Split("|");
        foreach (string field in fields)
        {
          var klassAndField = field.Split(".");
          var propertyName = klassAndField[1];
          switch (klassAndField[0])
          {
            case "PatientMeasurement":
              try
              {

                Type type = newWeightHeight.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                if (propertyName == "DateTaken")
                {
                  DateTime qDate = DateTime.ParseExact(propertyValue, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                  propertyInfo.SetValue(newWeightHeight, qDate);
                } else if (propertyName == "Height")
                {
                  newWeightHeight.Height = Convert.ToDecimal(propertyValue);
                }
                else if (propertyName == "Weight")
                {
                  newWeightHeight.Weight = Convert.ToDecimal(propertyValue);
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
