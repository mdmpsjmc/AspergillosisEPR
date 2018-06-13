using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public class ManARTSPatientSurgeryResolver
    {
        private string _surgeryCellValue;
        private IRow _row;
        private AspergillosisContext _context;
        private List<string> _headers;
        private static string _notesHeaderName = "LungSurgeryNotes";
        private static string _yearHeadersName = "LungSurgeryYear";

        public ManARTSPatientSurgeryResolver(AspergillosisContext context, 
                                             string surgeryCellValue, 
                                             IRow row, 
                                             List<string> headers)
        {
            _surgeryCellValue = surgeryCellValue;
            _context = context;
            _row = row;
            _headers = headers;
        }

        public PatientSurgery Resolve()
        {            
            var patientSurgery = new PatientSurgery();
            var surgeriesNames = _context.Surgeries.Select(s => s.Name).ToList();
            var surgeriesRegex = RegularExpressions.FindWordInList(surgeriesNames);
            var surgeryMatches = surgeriesRegex.Matches(_surgeryCellValue);
            var surgeryFieldYearMatches = RegularExpressions.ValidYear().Matches(_surgeryCellValue);

            if (surgeryMatches.Count > 0)
            {
                string matchedSurgeryName = surgeryMatches[0].ToString();
                Surgery dbSurgery = FindSurgeryByName(matchedSurgeryName);
                if (dbSurgery == null) return null;

                patientSurgery.SurgeryId = dbSurgery.ID;

                if (NotesCellValueIsNotBlank()) patientSurgery.Note = notesCellValue();
                FindAndSetYearInSurgeryColumns(patientSurgery, surgeryFieldYearMatches);
            }
            return patientSurgery;
        }

        private void FindAndSetYearInSurgeryColumns(PatientSurgery patientSurgery, MatchCollection surgeryFieldYearMatches)
        {
            if (FoundMatchInSurgeryColumn(surgeryFieldYearMatches))
            {
                int intYear = Int32.Parse(surgeryFieldYearMatches[0].ToString());
                patientSurgery.SurgeryDate = intYear;
            }
            else if (YearCellValueIsNotBlank())
            {
                var yearFieldYearMatches = RegularExpressions.ValidYear().Matches(yearCellValue());
                if (yearFieldYearMatches.Count > 0)
                {
                    int intYear = Int32.Parse(yearFieldYearMatches[0].ToString());
                    patientSurgery.SurgeryDate = intYear;
                }
            }
        }

        private static bool FoundMatchInSurgeryColumn(MatchCollection surgeryFieldYearMatches)
        {
            return surgeryFieldYearMatches.Count > 0;
        }

        private bool YearCellValueIsNotBlank()
        {
            return !string.IsNullOrEmpty(yearCellValue());
        }

        private bool NotesCellValueIsNotBlank()
        {
            return !string.IsNullOrEmpty(notesCellValue());
        }

        private Surgery FindSurgeryByName(string name)
        {
            return _context.Surgeries
                           .Where(s => s.Name.ToLower() == name.ToString().ToLower())
                           .FirstOrDefault();
        }

        private int notesHeaderIndex()
        {
            return _headers.IndexOf(_notesHeaderName);
        }

        private int yearHeaderIndex()
        {
            return _headers.IndexOf(_yearHeadersName);
        }

        private string notesCellValue()
        {
            return _row.GetCell(notesHeaderIndex(), MissingCellPolicy.RETURN_BLANK_AS_NULL)?.StringCellValue; 
        }

        private string yearCellValue()
        {
            string yearValue = "";
            var yearCell = _row.GetCell(yearHeaderIndex(), MissingCellPolicy.RETURN_BLANK_AS_NULL);
            if (yearCell != null)
            {
                if (yearCell != null && yearCell.CellType == CellType.Numeric)
                    yearValue = yearCell.NumericCellValue.ToString();
                else yearValue = yearCell.StringCellValue;
            }
            return yearValue;
        }
    }
}
