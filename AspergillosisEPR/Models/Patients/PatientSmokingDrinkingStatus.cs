using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public class PatientSmokingDrinkingStatus
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int SmokingStatusId { get; set; }
        public int? StartAge { get; set; }
        public int? StopAge { get; set; }
        public int? CigarettesPerDay { get; set; }
        public int? PacksPerYear { get; set; }
        public int? AlcolholUnits { get; set; }
        public bool AlcoholAbuse { get; set; }
    }
}
