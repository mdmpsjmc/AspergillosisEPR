using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientDrugLevel
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int DrugId { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public DateTime DateTaken { get; set; } 
        public DateTime DateReceived { get; set; }
        public decimal ResultValue { get; set; }
        public string ComparisionCharacter { get; set; }
        public string LabNumber { get; set; }

        public Drug Drug { get; set; }
        public Patient Patient { get; set; }
        public UnitOfMeasurement Unit { get; set; }
        

        public string Result()
        {
            if (!string.IsNullOrEmpty(ComparisionCharacter))
            {
                return ComparisionCharacter + ResultValue.ToString();
            } else
            {
                return ResultValue.ToString();
            }
        }

        public static List<string> ComparisionCharacters()
        {
            return new List<string>()
            {
                "<", ">"
            };
        }
    }
}
