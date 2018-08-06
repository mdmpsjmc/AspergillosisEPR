using AspergillosisEPR.Lib.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientAllergicIntoleranceItem : ISearchable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int AllergyIntoleranceItemId { get; set; }
        public string Severity { get; set; }
        [Column(TypeName = "ntext")]
        public string Note { get; set; }
        [Required]
        public string IntoleranceType { get; set; }
        public string AllergyIntoleranceItemType { get; set; }
        
        public Patient Patient { get; set; }
        [NotMapped]
        public string AllergicItemName { get; set; }
        public ICollection<PatientAllergicIntoleranceItemSideEffect> SideEffects { get; set; }

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

        public string SideEffectsNames
        {
            get
            {
                if (SideEffects != null && SideEffects.Count > 0)
                    return string.Join(", ", SideEffects.Select(x => x.SideEffect?.Name).ToList());
                else
                {
                    return "";
                }
            }
        }

        public string Descriptive()
        {
            string note = "";
            if (!string.IsNullOrEmpty(Severity)) note = note + Severity + " ";
            if (!string.IsNullOrEmpty(IntoleranceType)) note = note + IntoleranceType;
            if (!string.IsNullOrEmpty(AllergicItemName)) note = note + ": " + AllergicItemName.ToLower();
            return note;
        }

        public static List<string> Severities()
        {
            return new List<string>
            {
               "Mild", "Moderate","Severe", "Anaphylaxis"
            };
        }

        public static List<string> IntoleranceTypes()
        {
            return new List<string>
            {
                "Intolerance", "Allergy"
            };
        }

        public static List<string> AllergyIntoleranceItemTypes()
        {
            return new List<string>
            {
                "Drug", "Food", "Other", "Fungi"
            };
        }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "Severity", "PatientAllergicIntoleranceItem.Severity.Select" },
                { "Intolerance / Alergy to", "PatientAllergicIntoleranceItem.AllergyIntoleranceItemId.Select" }
            };
        }
    }
}