
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class PatientRadiologyFinding
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public DateTime DateTaken { get; set; }
        public int RadiologyTypeId { get; set; }
        public int FindingId { get; set; }
        public int ChestLocationId { get; set; }
        public int ChestDistributionId { get; set; }
        public int GradeId { get; set; }
        public int TreatmentResponseId { get; set; }
        [Column(TypeName = "ntext")]
        public string Note { get; set; }
    }
}
