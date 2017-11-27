using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace AspergillosisEPR.Models
{
    public class PatientDrug
    {
        [Key]
        public int ID { get; set; }
        public int PatientId { get; set; }
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
        public DateTime EndDate { get; set; }        
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

    }
}
