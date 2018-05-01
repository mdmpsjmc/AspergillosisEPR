using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using AspergillosisEPR.Models.SGRQDatabase;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RabbitConsumers.SGRQ
{
    class SGRQMananger
    {
        private List<RootObject> _messages;
        private AspergillosisContext _context;

        public SGRQMananger(List<RootObject> messages)
        {
            _messages = messages;
            _context = new AspergillosisContextFactory().CreateDbContext();
        }

        public List<PatientSTGQuestionnaire> GetObjects()
        {
            var questionnaires = new List<PatientSTGQuestionnaire>();
            foreach(var message in _messages)
            {
                foreach(var sgrq in message.sgrq)
                {
                    if (FindByOriginalId(sgrq.ID) == null)
                    {
                        var patient = _context.Patients
                                              .Where(p => p.RM2Number == sgrq.RM2Number())
                                              .FirstOrDefault();
                        if (patient == null)
                        {
                            AddPatientToTemporaryRM2List(sgrq);
                        }
                        else
                        {
                            var questionnaire = BuildPatientSTGQuestionnaire(patient, sgrq);
                            _context.PatientSTGQuestionnaires.Add(questionnaire);
                        }                       
                    }
                    else
                    {
                        continue;
                    }                    
                }
            }
            _context.SaveChanges();
            return questionnaires;
        }

        private void AddPatientToTemporaryRM2List(Sgrq sgrq)
        {            
        }

        private PatientSTGQuestionnaire FindByOriginalId(int iD)
        {
            return _context.PatientSTGQuestionnaires
                           .Where(sgrq => sgrq.OriginalImportedId == sgrq.ID.ToString())
                           .FirstOrDefault();
        }

        private PatientSTGQuestionnaire BuildPatientSTGQuestionnaire(Patient patient, Sgrq sgrq)
        {
            var questionnaire = new PatientSTGQuestionnaire()
            {
                PatientId = patient.ID, 
                ActivityScore = decimal.Parse(sgrq.ActivityScore.ToString()),
                SymptomScore = decimal.Parse(sgrq.SymptomScore.ToString()),
                ImpactScore = decimal.Parse(sgrq.ImpactScore.ToString()), 
                TotalScore = decimal.Parse(sgrq.TotalScore.ToString()),
                OriginalImportedId = sgrq.ID.ToString(),
                DateTaken = DateTime.ParseExact(sgrq.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };            
            return questionnaire;
        }          
    }
}
