using AspergillosisEPR.Lib.Search;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace AspergillosisEPR.Models
{
    public class PatientDrug : ISearchable
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Patient cannot be blank")]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Drug must be selected required")]
        public int DrugId { get; set; }

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

        public string DrugName {
            get
            {
                return Drug.Name;
            }
        }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "Drug Name", "PatientDrugs.Drug.Name" },
                { "Start Date", "PatientDrugs.Drug.StartDate" },
                { "End Date", "PatientDrugs.Drug.EndDate" }
            };
        }

    }
}
