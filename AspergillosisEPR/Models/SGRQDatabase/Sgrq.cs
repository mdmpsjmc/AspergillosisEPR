using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.SGRQDatabase
{
    public class Sgrq
    {
        public int ID { get; set; }
        public int Deceased { get; set; }
        public string NAC_ID { get; set; }
        public string Timestamp { get; set; }
        public string Date { get; set; }
        public int age { get; set; }
        public double weight { get; set; }
        public string mrc { get; set; }
        public object height { get; set; }
        public string PreLim { get; set; }
        public double SymptomsTotal { get; set; }
        public int ImpactTotal { get; set; }
        public double GrandTotal { get; set; }
        public double ActivityTotal { get; set; }
        public double SymptomScore { get; set; }
        public double ImpactScore { get; set; }
        public int ActivityScore { get; set; }
        public double TotalScore { get; set; }
        public object notes { get; set; }

        public string RM2Number()
        {
            return NAC_ID.Replace("CPA", String.Empty);
        }
    }
}
