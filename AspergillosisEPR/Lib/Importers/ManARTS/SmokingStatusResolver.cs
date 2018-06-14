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
        private PatientSmokingDrinkingStatus _smokingDrinkingStatus; 

        public SmokingStatusResolver(AspergillosisContext context, 
                                     string smokingStatus, IRow row, List<string> headers)
        {
            _context = context;
            _smokingStatus = smokingStatus;
            _row = row;
            _headers = headers;
            _smokingDrinkingStatus = new PatientSmokingDrinkingStatus();
        }

        internal PatientSmokingDrinkingStatus Resolve()
        {
            SetDbSmokingStatusId();
            SetSmokingStartAge();
            SetSmokingStopAge();
            SetCigarettesPerDay();
            SetAlcoholUnits();
            SetPackPerYears();
            return _smokingDrinkingStatus;
        }

        private void SetSmokingProperty(string propertyName, string stringCellValue)
        {
            var propertyInfo = _smokingDrinkingStatus.GetType().GetProperty(propertyName);
            if (propertyInfo.PropertyType == typeof(int?) || propertyInfo.PropertyType == typeof(int))
            {
                int propertyIntValue = Int32.Parse(stringCellValue);
                propertyInfo.SetValue(_smokingDrinkingStatus, propertyIntValue);
            }
            else if ((propertyInfo.PropertyType == typeof(double?) || propertyInfo.PropertyType == typeof(double)))
            {
                double propertyDoubleValue = Double.Parse(stringCellValue);
                propertyInfo.SetValue(_smokingDrinkingStatus, propertyDoubleValue);
            }
            else
            {
                propertyInfo.SetValue(_smokingDrinkingStatus, stringCellValue);
            }
        }

        private void SetDbSmokingStatusId()
        {
            var smokingStatusString = GetSmokingStatusName();
            if (!string.IsNullOrEmpty(smokingStatusString))
            {
                var dbSmokingStatus = _context.SmokingStatuses
                                              .Where(s => s.Name == smokingStatusString)
                                              .FirstOrDefault();
                if (dbSmokingStatus != null)
                {
                    _smokingDrinkingStatus.SmokingStatusId = dbSmokingStatus.ID;
                }
            }
        }

        private void SetSmokingDrinkingValue(int index, string propertyName)
        {
            var cell = _row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL);
            double cellValue;
            string stringCellValue;
            if (cell == null) return;
            if (cell.CellType == CellType.Numeric)
            {
                cellValue = cell.NumericCellValue;
                stringCellValue = cellValue.ToString();
            }
            else
            {
                stringCellValue = cell.StringCellValue;
            }
            if (!string.IsNullOrEmpty(stringCellValue)) SetSmokingProperty(propertyName, stringCellValue);
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

        private void SetSmokingStartAge()
        {
            var index = _headers.IndexOf(_smokingStartAgeHeader);
            SetSmokingDrinkingValue(index, "StartAge");
        }

        private void SetSmokingStopAge()
        {
            var index = _headers.IndexOf(_smokingStopAgeHeader);
            SetSmokingDrinkingValue(index, "StopAge");
        }

        private void SetCigarettesPerDay()
        {
            var index = _headers.IndexOf(_cigarettesPerDayHeader);
            SetSmokingDrinkingValue(index, "CigarettesPerDay");
        }

        private void SetAlcoholUnits()
        {
            var index = _headers.IndexOf(_alcoholUnitsHeader);
            SetSmokingDrinkingValue(index, "AlcolholUnits");
        }

        private void SetPackPerYears()
        {
            var index = _headers.IndexOf(_packsPerYearHeader);
            SetSmokingDrinkingValue(index, "PacksPerYear");
        }

    }
}
