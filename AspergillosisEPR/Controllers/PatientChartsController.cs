using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using AspergillosisEPR.Models.PatientViewModels;
using System.Linq;
using AspergillosisEPR.Helpers;

namespace AspergillosisEPR.Controllers
{
    public class PatientChartsController : Controller
    {
        private readonly AspergillosisContext _context;    
        public PatientChartsController(AspergillosisContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Sgrq(int patientId)
        {          
            var patient = await _context.Patients                                
                                .Include(p => p.STGQuestionnaires)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == patientId);

            var questionariesList = patient.STGQuestionnaires.OrderBy(q => q.DateTaken).ToList();
            var chartData = BuildSTRQViewModel(patient, questionariesList);
            return Json(chartData);
        }

        private List<PatientSGRQViewModel> BuildSTRQViewModel(Patient patient, ICollection<PatientSTGQuestionnaire> sTGQuestionnaires)
        {
            var questionariesChartData = new List<PatientSGRQViewModel>();
            foreach(var sgrq in sTGQuestionnaires)
            {                
                var viewModel = new PatientSGRQViewModel() {
                    DateTaken = DateHelper.DateTimeToUnixTimestamp(sgrq.DateTaken),
                    TotalScore = sgrq.TotalScore.ToString(),
                    PatientId = sgrq.PatientId,
                    RM2Number = patient.RM2Number,
                    PatientName = patient.FullName,
                    ActivityScore = sgrq.ActivityScore.ToString(),
                    ImpactScore = sgrq.ImpactScore.ToString(),
                    SymptomScore = sgrq.SymptomScore.ToString()
                };

                questionariesChartData.Add(viewModel);
            }
            
            return questionariesChartData;
        }
    }
}
