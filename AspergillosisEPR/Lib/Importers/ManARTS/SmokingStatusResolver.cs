using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.Patients;
using NPOI.SS.UserModel;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public class SmokingStatusResolver
    {
        private AspergillosisContext _context;
        private string _smokingStatus;
        private IRow _row;
        private List<string> _headers;
        private string _smokingStartAgeHeader = "SmokingStartAge";
        private string _smokingStopAgeHeader = "SmokingStopAge";
        private string _cigarettesPerDayHeader = "CigsPD";
        private string _alcoholUnitsHeader = "AlcoholUnits";
        private string _packsPerYearHeader = "PackYrs";

        public SmokingStatusResolver(AspergillosisContext context, 
                                     string smokingStatus, IRow row, List<string> headers)
        {
            _context = context;
            _smokingStatus = smokingStatus;
            _row = row;
            _headers = headers;
        }

        internal PatientSmokingDrinkingStatus Resolve()
        {
            var smokingDrinkingStatus = new PatientSmokingDrinkingStatus();
            SetDbSmokingStatusId(smokingDrinkingStatus);
            SetSmokingStartAge(smokingDrinkingStatus);
            SetSmokingStopAge(smokingDrinkingStatus);
            SetCigarettesPerDay(smokingDrinkingStatus);
            SetAlcoholUnits(smokingDrinkingStatus);
            return smokingDrinkingStatus;
        }

        private void SetDbSmokingStatusId(PatientSmokingDrinkingStatus smokingDrinkingStatus)
        {
            var smokingStatusString = GetSmokingStatusName();
            if (!string.IsNullOrEmpty(smokingStatusString))
            {
                var dbSmokingStatus = _context.SmokingStatuses
                                              .Where(s => s.Name == smokingStatusString)
                                              .FirstOrDefault();
                if (dbSmokingStatus != null)
                {
                    smokingDrinkingStatus.SmokingStatusId = dbSmokingStatus.ID;
                }
            }
        }

        private void SetSmokingStartAge(PatientSmokingDrinkingStatus smokingDrinkingStatus)
        {
            var index = _headers.IndexOf(_smokingStartAgeHeader);
            var cell = _row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL);
            if (cell != null)
            {
                string cellValue = cell.StringCellValue;
                if (!string.IsNullOrEmpty(cellValue)) smokingDrinkingStatus.StartAge = Int32.Parse(cellValue);
            }
        }

        private void SetSmokingStopAge(PatientSmokingDrinkingStatus smokingDrinkingStatus)
        {
            var index = _headers.IndexOf(_smokingStopAgeHeader);
            var cell = _row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL);
            if (cell != null)
            {
                string cellValue = cell.StringCellValue;
                if (!string.IsNullOrEmpty(cellValue)) smokingDrinkingStatus.StopAge = Int32.Parse(cellValue);
            }
        }

        private void SetCigarettesPerDay(PatientSmokingDrinkingStatus smokingDrinkingStatus)
        {
            var index = _headers.IndexOf(_cigarettesPerDayHeader);
            var cell = _row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL);
            if (cell != null)
            {
                string cellValue = cell.StringCellValue;
                if (!string.IsNullOrEmpty(cellValue)) smokingDrinkingStatus.CigarettesPerDay = Int32.Parse(cellValue);
            }
        }

        private void SetAlcoholUnits(PatientSmokingDrinkingStatus smokingDrinkingStatus)
        {
            var index = _headers.IndexOf(_alcoholUnitsHeader);
            var cell = _row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL);
            if (cell != null)
            {
                string cellValue = cell.StringCellValue;
                if (!string.IsNullOrEmpty(cellValue)) smokingDrinkingStatus.AlcolholUnits = Int32.Parse(cellValue);
            }
        }

        private string GetSmokingStatusName()
        {
            switch (_smokingStatus)
            {
                case "x":
                    return "Ex-Smoker";
                case "0":
                    return "Never";
                case "1":
                    return "Current";
                case "3":
                    return "Don't know";
                default:
                    return null;
            }
        }

    }
}
