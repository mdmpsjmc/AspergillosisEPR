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
  public class MortalityAuditDatesSGRQImporter : SpreadsheetImporter
    {
  
    public static string[] IdentifierHeaders = { "RM2Number" };
    public MortalityAuditDatesSGRQImporter(FileStream stream, IFormFile file,
                             string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

    {
    }

    public static Hashtable HeadersDictionary()
    {
      return new Hashtable()
      {
                  { "RM2Number", "Patient.RM2Number" },
                  { "SymptomScore", "PatientSTGQuestionnaire.SymptomScore"},
                  { "ImpactScore", "PatientSTGQuestionnaire.ImpactScore"},
                  { "ActivityScore", "PatientSTGQuestionnaire.ActivityScore"},
                  { "TotalScore", "PatientSTGQuestionnaire.TotalScore"},
                  { "DateTaken", "PatientSTGQuestionnaire.DateTaken"},
      };
    }

    protected override void ProcessSheet(ISheet currentSheet)
    {
      Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
      {
        List<PatientSTGQuestionnaire> sgrqs = _context.PatientSTGQuestionnaires.Where(d => d.PatientId.Equals(patient.ID)).ToList();
        var newSgrq = new PatientSTGQuestionnaire() { PatientId = patient.ID };
        patient = ReadRowCellsIntoPatientObject(patient, row, cellCount, newSgrq);
        var existingDates = sgrqs.Select(pi => pi.DateTaken.Date).ToList();        
        bool dateDoesNotExist = existingDates.FindAll(d => d.Date == newSgrq.DateTaken.Date).ToList().Count == 0;
        if (newSgrq.DateTaken.Year > 1 && dateDoesNotExist)
        {

          if (patient.STGQuestionnaires == null) patient.STGQuestionnaires = new List<PatientSTGQuestionnaire>();
          patient.STGQuestionnaires.Add(newSgrq);

        }
        Imported.Add(patient);
      };
      InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
    }

    private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount, PatientSTGQuestionnaire newSgrq)
    {
      for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
      {
        if (row.GetCell(cellCursor) != null)
        {
         
          ReadCell(patient, row, cellCursor, newSgrq);      
        }
      }

      return patient;
    }

    private void ReadCell(Patient patient, IRow row, int cellIndex, PatientSTGQuestionnaire newSGRQ)
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
            case "PatientSTGQuestionnaire":
              try
              {

                Type type = newSGRQ.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                if (propertyName == "DateTaken")
                {
                  DateTime qDate = DateTime.ParseExact(propertyValue, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                  propertyInfo.SetValue(newSGRQ, qDate);
                } else if (propertyName == "SymptomScore")
                {
                  newSGRQ.SymptomScore = Convert.ToDecimal(propertyValue);
                } else if (propertyName == "ImpactScore")
                {
                  newSGRQ.ImpactScore = Convert.ToDecimal(propertyValue);
                }
                else if (propertyName == "ActivityScore")
                {
                  newSGRQ.ActivityScore = Convert.ToDecimal(propertyValue);
                }
                else if (propertyName == "TotalScore")
                {
                  newSGRQ.TotalScore = Convert.ToDecimal(propertyValue);
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
