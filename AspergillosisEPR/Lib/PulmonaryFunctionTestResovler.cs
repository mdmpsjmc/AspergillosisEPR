using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.Spirometry;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class PulmonaryFunctionTestResovler
    {
        public string Age { get; set; }
        public decimal Height { get; set; }
        public string DLCOValue { get; set; }
        public string FEV1Value { get; set; }
        public string FVCValue { get; set; }
        public string KCOValue { get; set; }
        public string DateTaken { get; set; }
        public string VAValue { get; set; }
        public static List<string> Tests = new List<string>() { "VA", "DLCO", "FEV1", "FVC", "KCO"};

        public List<PatientPulmonaryFunctionTest> ResolvePFTs(AspergillosisContext context, Patient patient)
        {
            var pfts = new List<PatientPulmonaryFunctionTest>();
            foreach(var test in Tests)
            {
                try
                {
                    var patientPulmonaryFunctionTest = new PatientPulmonaryFunctionTest();
                    var dbTest = context.PulmonaryFunctionTests.Where(pft => pft.ShortName.Equals(test)).FirstOrDefault();
                    if (dbTest == null) continue;
                    patientPulmonaryFunctionTest.PulmonaryFunctionTest = dbTest;
                    patientPulmonaryFunctionTest.Patient = patient;
                    var property = GetType().GetProperty(test + "Value");
                    string testValue = property.GetValue(this, null)?.ToString();
                    if (testValue == null) continue;
                    var decimalTest = Convert.ToDecimal(testValue);
                    patientPulmonaryFunctionTest.DateTaken = (DateTime)Convert.ToDateTime(DateTaken);
                    patientPulmonaryFunctionTest.ResultValue = decimalTest;
                    var calculator = GetCalculatorForPatient(test, patient);
                    if (calculator != null)
                    {
                        var predictedValue = calculator.Calculate(AgeAsInteger(), HeightAsDouble());
                        var predicted = calculator.Percentage(AgeAsInteger(), HeightAsDouble(), Convert.ToDouble(decimalTest));
                        patientPulmonaryFunctionTest.PredictedValue = Convert.ToDecimal(predicted);
                        patientPulmonaryFunctionTest.NormalValue = predictedValue;
                        patientPulmonaryFunctionTest.Age = AgeAsInteger();
                        patientPulmonaryFunctionTest.Height = Convert.ToInt32(HeightAsDouble() * 100);
                    }
                    patientPulmonaryFunctionTest.CreatedDate = DateTime.Now;
                    pfts.Add(patientPulmonaryFunctionTest);
                } catch (OverflowException ex)
                {
                    continue;
                }
            }
            return pfts;
        }

        private int AgeAsInteger()
        {
            return Convert.ToInt32(Age);
        }

        private double HeightAsDouble()
        {
            return Convert.ToDouble(Height);
        }

        private Equation GetCalculatorForPatient(string pft, Patient patient)
        {
            List<Equation> collection = new List<Equation>();
            if (patient.Gender.ToLowerInvariant() == "male")
            {
                collection = RegressionEquations.Males;
            } else if (patient.Gender.ToLowerInvariant() == "female")
            {
                collection = RegressionEquations.Females;
            }
            var calc = collection.Where(test => test.PFT.Equals(pft)).FirstOrDefault();
            return calc;
        }
    }
}
