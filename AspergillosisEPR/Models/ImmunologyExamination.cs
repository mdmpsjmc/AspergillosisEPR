using AspergillosisEPR.Models.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class ImmunologyExamination : PatientExamination
    {
        public string AsDetailedString()
        {
            string detailedString = "Date: " + PatientImmunoglobulin.DateTaken.ToString("dd-MM-yyyy");
            detailedString += " " +  PatientImmunoglobulin.ImmunoglobulinType.Name + " ";
            detailedString += " = " + PatientImmunoglobulin.Value.ToString();
            return detailedString;
        }
    }
}