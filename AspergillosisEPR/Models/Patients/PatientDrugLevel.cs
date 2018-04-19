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

        public static List<string> ComparisionCharacters()
        {
            return new List<string>()
            {
                "<", ">"
            };
        }
    }
}
