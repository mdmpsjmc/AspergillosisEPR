using AspergillosisEPR.Lib.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientSmokingDrinkingStatus : ISearchable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int SmokingStatusId { get; set; }
        public int? StartAge { get; set; }
        public int? StopAge { get; set; }
        public double? CigarettesPerDay { get; set; }
        public double? PacksPerYear { get; set; }
        public double? AlcolholUnits { get; set; }

        public SmokingStatus SmokingStatus { get; set; }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "SmokingStatus", "PatientSmokingDrinkingStatus.SmokingStatusId.Select" }
            };
        }
    }
}
