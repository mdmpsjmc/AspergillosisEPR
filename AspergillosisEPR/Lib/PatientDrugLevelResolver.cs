using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class PatientDrugLevelResolver
    {
        private AspergillosisContext _context;        
        public PatientDrugLevel PatientDrugLevel;


        public PatientDrugLevelResolver(AspergillosisContext context, Drug drug, UnitOfMeasurement uom)
        {
            _context = context;
            PatientDrugLevel = new PatientDrugLevel()
            {
                Drug = drug,
                Unit = uom
            };                        
        }

        public void SetProperty(string propertyName, string propertyValue)
        {
            Type patientDrugLevelType = PatientDrugLevel.GetType();
            PropertyInfo property = patientDrugLevelType.GetProperty(propertyName);
            if (propertyName.Contains("Date"))
            {
                string format = "dd-MMM-yyyy";
                DateTime result = DateTime.ParseExact(propertyValue, format, CultureInfo.InvariantCulture);
                property.SetValue(PatientDrugLevel, result);
            }
            else
            {
                if (propertyName.Contains("ResultValue"))
                {
                    SetResult(propertyValue, patientDrugLevelType, property);
                }
                else
                {
                    property.SetValue(PatientDrugLevel, Convert.ChangeType(propertyValue, property.PropertyType), null);
                }
            }
        }     

        public Patient Resolve(Patient patient)
        {
            if (patient.DrugLevels == null || patient.DrugLevels.Count == 0)
            {
                patient.DrugLevels = new List<PatientDrugLevel>();
            }
            patient.DrugLevels.Add(PatientDrugLevel);
            return patient;
        }

        private void SetResult(string propertyValue, Type patientDrugLevelType, PropertyInfo property)
        {
            Regex regex = new Regex(@">|<");
            Match match = regex.Match(propertyValue);
            if (match.Success)
            {
                string comparisionChar = match.Captures[0].ToString();
                PropertyInfo comparisionCharacterProperty = patientDrugLevelType.GetProperty("ComparisionCharacter");
                comparisionCharacterProperty.SetValue(PatientDrugLevel, comparisionChar);
                decimal parsedValue = decimal.Parse(propertyValue.Replace(comparisionChar, String.Empty));
                property.SetValue(PatientDrugLevel, parsedValue);
            }
            else
            {
                property.SetValue(PatientDrugLevel, decimal.Parse(propertyValue));
            }
        }
    }
}
