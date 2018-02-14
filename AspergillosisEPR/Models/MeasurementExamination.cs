using AspergillosisEPR.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class MeasurementExamination : PatientExamination
    {

        public string  AsDetailedString()
        {
            string detailedString = "Date: " + PatientMeasurement.DateTaken.ToString("dd-MM-yyyy");
            if (PatientMeasurement.Weight != null)
            {
                detailedString += " Weight: " + PatientMeasurement.Weight.ToString();
            }
            if (PatientMeasurement.Height != null)
            {
                detailedString += " Height: " + PatientMeasurement.Height.ToString();
            }
            return detailedString;
        }
    }
}
