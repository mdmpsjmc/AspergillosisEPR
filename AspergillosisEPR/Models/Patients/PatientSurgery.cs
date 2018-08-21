using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Lib.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientSurgery : Exportable, ISearchable
    {
        public int ID { get; set; }
        public int SurgeryId { get; set; }
        public int PatientId { get; set; }
        public int? SurgeryDate { get; set; }
        public string Note { get; set; }

        public Surgery Surgery { get; set; }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "Surgery Name", "PatientSurgery.SurgeryId.Select" }
            };
        }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient", "Surgery"
            };
        }

    }
}
