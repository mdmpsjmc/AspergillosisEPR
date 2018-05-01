using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.SGRQDatabase
{
    public class Sgrq
    {
        public int ID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Deceased { get; set; }
        public string NAC_ID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Date { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string age { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double weight { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mrc { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double height { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreLim { get; set; }
        public double SymptomsTotal { get; set; }
        public double ImpactTotal { get; set; }
        public double GrandTotal { get; set; }
        public double ActivityTotal { get; set; }
        public double SymptomScore { get; set; }
        public double ImpactScore { get; set; }
        public double ActivityScore { get; set; }
        public double TotalScore { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string notes { get; set; }

        public string RM2Number()
        {
            return NAC_ID.Replace("CPA", String.Empty);
        }
    }
}
