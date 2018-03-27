
using AspergillosisEPR.Models.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class RadiologyExamination : PatientExamination
    {
        public string AsDetailedString()
        {
            string detailedString = "Date: " + PatientRadiologyFiniding.DateTaken.ToString("dd-MM-yyyy");
            detailedString = detailedString + PatientRadiologyFiniding.Appearance;
            return detailedString;
        }
    }
}
