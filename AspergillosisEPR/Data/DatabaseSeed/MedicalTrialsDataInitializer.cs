using AspergillosisEPR.Models;
using AspergillosisEPR.Models.MedicalTrials;
using AspergillosisEPR.Models.Radiology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed
{
    public class MedicalTiralsDataInitializer
    {
        public static  void AddMedicalTrialsModels(AspergillosisContext context)
        {
            if (context.MedicalTrialStatuses.Any())
            {
                return;
            }

            var personTitles = new PersonTitle[]
            {
                new PersonTitle { Name = "Professor"},
                new PersonTitle { Name = "Dr."},
                new PersonTitle { Name = "Mr" },
                new PersonTitle { Name = "Mrs"},
                new PersonTitle { Name = "Ms" },
                new PersonTitle { Name = "Rev." }
            };

            foreach (var title in personTitles)
            {
                context.Add(title);
            }

            var patientMedicalTrialsStatuses = new MedicalTrialPatientStatus[]
            {
                new MedicalTrialPatientStatus { Name = "Identified Not Yet Assigned"},
                new MedicalTrialPatientStatus { Name = "In Screening"},
                new MedicalTrialPatientStatus { Name = "Not Suitable" },
                new MedicalTrialPatientStatus { Name = "Declined"},
                new MedicalTrialPatientStatus { Name = "Failure" },
                new MedicalTrialPatientStatus { Name = "Recruited On Study"},
                new MedicalTrialPatientStatus { Name = "Completed Trial"},
                new MedicalTrialPatientStatus { Name = "In Follow Up" },
                new MedicalTrialPatientStatus { Name = "Withdrawn"},
                new MedicalTrialPatientStatus { Name = "Deceased"},
                new MedicalTrialPatientStatus { Name = "Flagged for check"},
                new MedicalTrialPatientStatus { Name = "Did NOT Attend"}
            };

            foreach (var status in patientMedicalTrialsStatuses)
            {
                context.Add(status);
            }

            var medicalTrialStatuses = new MedicalTrialStatus[]
           {
                new MedicalTrialStatus { Name = "Awaiting Approval"},
                new MedicalTrialStatus { Name = "Closed To Recruitment"},
                new MedicalTrialStatus { Name = "Closed To Recruitment in Follow Up" },
                new MedicalTrialStatus { Name = "Complete"},
                new MedicalTrialStatus { Name = "Follow Up Only" },
                new MedicalTrialStatus { Name = "Open To Recruitment" }
           };

            foreach (var status in medicalTrialStatuses)
            {
                context.Add(status);
            }

            var trialTypes = new MedicalTrialType[]
            {
                new MedicalTrialType { Name = "Randomized controlled trial"},
                new MedicalTrialType { Name = "Non Randomized controlled trial"}
            };

            foreach (var type in trialTypes)
            {
                context.Add(type);
            }

            context.SaveChanges();
        }
    }
}
