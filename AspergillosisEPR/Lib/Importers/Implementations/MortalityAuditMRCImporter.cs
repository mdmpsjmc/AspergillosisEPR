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
  public class MortalityAuditMRCImporter : SpreadsheetImporter
    {
  
    public static string[] IdentifierHeaders = { "RM2Number" };
    public MortalityAuditMRCImporter(FileStream stream, IFormFile file,
                             string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

    {
    }

    public static Hashtable HeadersDictionary()
    {
      return new Hashtable()
      {
                  { "RM2Number", "Patient.RM2Number" },
                  { "Score", "PatientMRCScore.Score"},
                  { "DateTaken", "PatientMRCScore.DateTaken"},
      };
    }

    protected override void ProcessSheet(ISheet currentSheet)
    {
      Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
      {
        List<PatientMRCScore> mrcs = _context.PatientMRCScores.Where(d => d.PatientId.Equals(patient.ID)).ToList();
        var newMRC = new PatientMRCScore() { PatientId = patient.ID };
        patient = ReadRowCellsIntoPatientObject(patient, row, cellCount, newMRC);
        var existingDates = mrcs.Select(pi => pi.DateTaken.Date).ToList();        
        bool dateDoesNotExist = existingDates.FindAll(d => d.Date == newMRC.DateTaken.Date).ToList().Count == 0;
        if (newMRC.DateTaken.Year > 1 && dateDoesNotExist)
        {

          if (patient.PatientMRCScores == null) patient.PatientMRCScores = new List<PatientMRCScore>();
          patient.PatientMRCScores.Add(newMRC);

        }
        Imported.Add(patient);
      };
      InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
    }

    private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount, PatientMRCScore newMRC)
    {
      for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
      {
        if (row.GetCell(cellCursor) != null)
        {
         
          ReadCell(patient, row, cellCursor, newMRC);      
        }
      }

      return patient;
    }

    private void ReadCell(Patient patient, IRow row, int cellIndex, PatientMRCScore newMRC)
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
            case "PatientMRCScore":
              try
              {

                Type type = newMRC.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                if (propertyName == "DateTaken")
                {
                  DateTime qDate = DateTime.ParseExact(propertyValue, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                  propertyInfo.SetValue(newMRC, qDate);
                } else if (propertyName == "Score")
                {
                  newMRC.Score = propertyValue;
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
