using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class TestType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int UnitOfMeasurementId { get; set; }

        public static Dictionary<string, string> Units()
        {
            var codes = new Dictionary<string, string>();
            codes.Add("LY", "x10^9/l");
            codes.Add("HB", "g/l");
            codes.Add("CRP", "mg/L");
            codes.Add("WBC", "x10^9/l");
            codes.Add("ALB", "g/L");
            codes.Add("MBL", "mg/L");
            codes.Add("ALT", "IU/L");
            codes.Add("PLT", "x10^9/l");
            codes.Add("EO", "x10^9/l");
            codes.Add("NE", "x10^9/l");
            codes.Add("K", "mmol/L");
            codes.Add("NA", "mmol/L");
            codes.Add("GLOB", "g/L");
            codes.Add("TBILI", "umol/L");
            codes.Add("ALP", "IU/L");
            codes.Add("CREA", "umol/L");
            codes.Add("EGFR", "mL/min/1.73m2");
            return codes;
        }

        public static Dictionary<string, string> Codes()
        {
            var codes = new Dictionary<string, string>();
            codes.Add("LY", "Lymphocytes");
            codes.Add("HB", "Haemoglobin");
            codes.Add("CRP", "C-Reactive Protein (CRP)");
            codes.Add("WBC", "WBC");
            codes.Add("ALB", "Albumin");
            codes.Add("MBL", "Mannose Binding Lectin");
            codes.Add("ALT", "ALT");
            codes.Add("PLT", "Platelets");
            codes.Add("EO", "Eosinophils");
            codes.Add("NE", "Neutrophils");
            codes.Add("K", "Potassium");
            codes.Add("NA", "Sodium");
            codes.Add("GLOB", "Globulin");
            codes.Add("TBILI", "Total Bilirubin");
            codes.Add("ALP", "Alk.Phos");
            codes.Add("CREA", "Creatinine");
            codes.Add("EGFR", "Estimated GFR (CKD-EPI)");
            return codes;
        }

        public static string LabTestFromCode(string code)
        {
            return Codes()[code];
        }


    }
}
