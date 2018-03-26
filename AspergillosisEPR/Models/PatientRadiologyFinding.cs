
using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Lib.Search;
using AspergillosisEPR.Models.Radiology;
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
    public class PatientRadiologyFinding : Exportable, ISearchable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        [Display(Name = "Date Taken")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
        public DateTime DateTaken { get; set; }
        public int RadiologyTypeId { get; set; }
        public int FindingId { get; set; }
        public int ChestLocationId { get; set; }
        public int ChestDistributionId { get; set; }
        public int GradeId { get; set; }
        public int TreatmentResponseId { get; set; }
        [Column(TypeName = "ntext")]
        public string Note { get; set; }

        public RadiologyType RadiologyType { get; set; }
        public Finding Finding { get; set; }
        public ChestLocation ChestLocation { get; set; }
        public ChestDistribution ChestDistribution { get; set; }
        public Grade Grade { get; set; }
        public TreatmentResponse TreatmentResponse { get; set; }

        public string Appearance
        {
            get
            {
                if (Grade == null || Finding == null || ChestLocation == null || ChestDistribution == null || TreatmentResponse == null)
                {
                    return "";
                } else
                {
                    string text = "";
                    text = Grade.Name + " " + Finding.Name + " location:" + ChestLocation.Name + ", " + ChestDistribution.Name;
                    text = text + ". Treatment Response: " + TreatmentResponse.Name;
                    return text;
                }         
            }
        }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient", "RadiologyType",
                "RadiologyTypeId", "Finding", "FindingId",
                "ChestLocation", "ChestLocationId", "ChestDistribution",
                "ChestDistributionId", "Grade", "GradeId", 
                "TreatmentResponse", "TreatmentResponseId"
            };
        }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "Radiology Finding", "PatientRadiologyFinding.FindingId.Select"},
                { "Radiology Type", "PatientRadiologyFinding.RadiologyTypeId.Select"},
                { "Chest Distribution", "PatientRadiologyFinding.ChestDistributionId.Select"},
                { "Chest Location", "PatientRadiologyFinding.ChestLocationId.Select"},
                { "Grade", "PatientRadiologyFinding.GradeId.Select"},
                { "Treatment Response", "PatientRadiologyFinding.TreatmentResponseId.Select"}
            };
        }
    }
}
