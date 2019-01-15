using AspergillosisEPR.Models.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Spirometry
{
    public class SpirometryCalculator
    {
        private PatientPulmonaryFunctionTest _patientPulmonaryFunctionTest;
        private List<Equation> _equations;
        private string _pft;
        private int _age;
        private int _hegiht;
        private string _gender;

        public SpirometryCalculator(PatientPulmonaryFunctionTest patientPulmonaryFunctionTest)
        {
            _patientPulmonaryFunctionTest = patientPulmonaryFunctionTest;
            SetEquationList();
        }

        public SpirometryCalculator(string pft, int age, int height, string gender)
        {
            _pft = pft;
            _age = age;
            _hegiht = height;
            _gender = gender;
            SetEquationListFromGender();
        }

        public double ERSPredictedValue(int height)
        {
            if (_patientPulmonaryFunctionTest == null) return 0.0;
            var equation = _equations.Where(eq => eq.PFT.Equals(_patientPulmonaryFunctionTest.PulmonaryFunctionTest.ShortName))
                                     .FirstOrDefault();
            return equation.Calculate(_patientPulmonaryFunctionTest.Patient.Age(), height);
        }

        public double ERSPredictedValue()
        {
            var equation = _equations.Where(eq => eq.PFT.Equals(_pft))
                                     .FirstOrDefault();
            return equation.Calculate(_age, _hegiht);
        }

        private void SetEquationList()
        {
            if (_patientPulmonaryFunctionTest.Patient.Gender.ToLowerInvariant() == "male")
            {
                _equations = RegressionEquations.Males;
            } else
            {
                _equations = RegressionEquations.Females;
            }
        }

        private void SetEquationListFromGender()
        {
            if (_gender == "male")
            {
                _equations = RegressionEquations.Males;
            }
            else
            {
                _equations = RegressionEquations.Females;
            }
        }

    }
}
