using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class PatientSTGQuestionnaireResolver
    {
        private AspergillosisContext _context;
        private List<PatientSTGQuestionnaire> _questionnaires;
        private Dictionary<string, string> _fields;
        private PatientSTGQuestionnaire _questionnaire;

        public PatientSTGQuestionnaireResolver(AspergillosisContext context)
        {
            _context = context;
            _questionnaires = new List<PatientSTGQuestionnaire>();
            _fields = new Dictionary<string, string>();
            _questionnaire = new PatientSTGQuestionnaire();
        }

        public List<PatientSTGQuestionnaire> Resolve()
        {           
            _questionnaires.Add(_questionnaire);
            return _questionnaires;
        }

        public void AddField(string field, string value)
        {
            _fields.Add(field, value);
        }

        public  void SetQuestionnaireProperty(string property, string propertyValue)
        {
            if (_questionnaire == null)
            {
                _questionnaire = new PatientSTGQuestionnaire();
            }
            Type type = new PatientSTGQuestionnaire().GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);
            DateTime dateRowValue;
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime))
            { 
                try
                {
                    DateTime.TryParse(propertyValue, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dateRowValue);
                    propertyInfo.SetValue(_questionnaire, dateRowValue);
                } catch(FormatException ex)
                {
                }
                
            }
            else if ((propertyInfo != null) && (propertyInfo.PropertyType == typeof(decimal)))
            {
                propertyInfo.SetValue(_questionnaire, decimal.Parse(propertyValue));
            }
        }
    }
}
