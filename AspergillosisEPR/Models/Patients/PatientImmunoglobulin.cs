using AspergillosisEPR.Lib.Exporters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientImmunoglobulin : Exportable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int ImmunoglobulinTypeId { get; set; }
        public string SampleId { get; set; }
        public string Range { get; set; }
        [Display(Name = "Date Taken")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }
        public decimal Value { get; set;  }
        public ImmunoglobulinType ImmunoglobulinType { get; set; }
        public decimal? SourceSystemGUID { get; set; } = 0;
        public DateTime? CreatedDate { get; set; }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient", "ImmunoglobulinType", "SampleId", "Range"
            };
        }


        public static string IgFromCode(string code)
        {
            return Codes()[code];
        }

        public static Dictionary<string, string> Codes()
        {
            var codes = new Dictionary<string, string>();
            codes.Add("ASPIGG", "Aspergillus F IgG");
            codes.Add("ASPIGE", "Aspergillus F IgE");
            codes.Add("IGEX2", "Total IgE");
            codes.Add("WIGM", "IgM");
            codes.Add("WIGA", "IgA");
            return codes;
        }

    }
}
