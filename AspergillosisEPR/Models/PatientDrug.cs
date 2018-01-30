using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Lib.Search;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AspergillosisEPR.Models
{
    public class PatientDrug : Exportable, ISearchable
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Patient cannot be blank")]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Drug must be selected required")]
        public int DrugId { get; set; }
        [JsonIgnore]
        public Patient Patient { get; set; }
        public Drug Drug { get; set; }
        public ICollection<PatientDrugSideEffect> SideEffects { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? EndDate { get; set; }
        public List<int> SelectedEffectsIds
        {
            get
            {
                if (SideEffects != null)
                    return SideEffects.Select(x => x.SideEffectId).ToList();
                else
                {
                    return new List<int>();
                }
            }
        }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient", "SideEffects", "SelectedEffectsIds", "Drug"
            };
        }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "Drug Name", "PatientDrugs.DrugId.Select" },
                { "Start Date", "PatientDrugs.Drug.StartDate" },
                { "End Date", "PatientDrugs.Drug.EndDate" }
            };
        }

    }
}
