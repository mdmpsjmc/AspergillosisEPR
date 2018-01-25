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
using System.IO;
using Microsoft.AspNetCore.NodeServices;

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
            var chartData = PatientSGRQViewModel.Build(patient, questionariesList);
            return Json(chartData);
        }        
    }
}
