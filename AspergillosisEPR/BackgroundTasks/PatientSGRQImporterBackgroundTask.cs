// Modded by JC for District Numbers 01/02/2021
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.BackgroundTasks
{
  public class PatientSGRQImporterBackgroundTask : ScheduledProcessor
  {
    protected override string Schedule => "15 12 * * 6"; //every sat at 12:15
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PatientSGRQImporterBackgroundTask> _logger;

    public PatientSGRQImporterBackgroundTask(IServiceScopeFactory serviceScopeFactory,
                                             IServiceProvider serviceProvider,
                                             ILogger<PatientSGRQImporterBackgroundTask> logger) : base(serviceScopeFactory)
    {
      _logger = logger;
      _serviceProvider = serviceProvider;
    }

    public override Task ProcessInScope(IServiceProvider serviceProvider)
    {

      using (IServiceScope scope = _serviceProvider.CreateScope())
      {
        var context = scope.ServiceProvider.GetRequiredService<AspergillosisContext>();
        var sgrqContext = scope.ServiceProvider.GetRequiredService<SGRQContext>();
        var lastImportedSGRQ = context.PatientSTGQuestionnaires.OrderByDescending(q => q.DateTaken).FirstOrDefault();

        var sqrqs = sgrqContext.QuestionnaireResults
                                    .Include(q => q.Questionnaire)
                                      .ThenInclude(q => q.Patient)
                                    .Where(q => q.Questionnaire.ID > Int32.Parse(lastImportedSGRQ.OriginalImportedId));


        foreach (var externalSGRQ in sqrqs)
        {
          var districtNumber = externalSGRQ.Questionnaire.Patient.Identifier.ToString();
          var patient = context.Patients
                                .Include(p => p.STGQuestionnaires)
                                .Where(p => p.DistrictNumber.Equals(districtNumber)).FirstOrDefault();

          if (patient == null) continue;

          var existingDates = patient.STGQuestionnaires.Select(pi => pi.DateTaken.Date).ToList();
          bool dateDoesNotExist = existingDates.FindAll(d => d.Date == externalSGRQ.Questionnaire.DateOfEntry.Date).ToList().Count == 0;

          if (dateDoesNotExist)
          {

            var sgrq = new PatientSTGQuestionnaire();
            sgrq.PatientId = patient.ID;
            sgrq.SymptomScore = (decimal)externalSGRQ.SymptomScore;
            sgrq.ImpactScore = (decimal)externalSGRQ.ImpactScore;
            sgrq.ActivityScore = (decimal)externalSGRQ.ActivityScore;
            sgrq.TotalScore = (decimal)externalSGRQ.TotalScore;
            sgrq.DateTaken = externalSGRQ.Questionnaire.DateOfEntry;
            sgrq.OriginalImportedId = externalSGRQ.Questionnaire.ID.ToString();
            context.PatientSTGQuestionnaires.Add(sgrq);
          }
        }

        _logger.LogInformation("Import Finished at: " + DateTime.Now.ToString());
        context.SaveChanges();
        return Task.CompletedTask;
      }
    }
  }
}
