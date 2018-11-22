using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.ExternalImportDb
{
    public class Diagnosis
    {
        public int ID { get; set; }
        public string RM2Number { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string DiagnosisCode { get; set; }
        [Column(TypeName = "text")]
        public string DiagnosisDescription { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
