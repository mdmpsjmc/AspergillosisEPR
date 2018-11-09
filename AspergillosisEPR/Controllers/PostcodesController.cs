using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.BackgroundTasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspergillosisEPR.Controllers
{
    public class PostcodesController : Controller
    {
        private AspergillosisContext _context;
        private PASDbContext _pasDbContext;
        private ILogger _logger;
        private IBackgroundTaskQueue Queue;
        private IApplicationLifetime _appLifetime;

        public PostcodesController(AspergillosisContext context, 
                                    PASDbContext pasDbContext,
                                    ILogger<PostcodesController> logger,
                                    IBackgroundTaskQueue queue,
                                    IApplicationLifetime appLifetime
                                    ) 
        {
            _context = context;
             Queue = queue;
            _pasDbContext = pasDbContext;
            _appLifetime = appLifetime;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Update()
        {
            var patientsWithoutPostCodes = _context.Patients; //.Where(p => p.PostCode == null);

            foreach(var patientWithoutPostCode in patientsWithoutPostCodes)
            {
                var lpiPatientData = _pasDbContext.LpiPatientData.FirstOrDefault(p => p.RM2Number() == patientWithoutPostCode.RM2Number);
                if (lpiPatientData == null || lpiPatientData.POSTCODE == null) continue;                
                patientWithoutPostCode.PostCode = lpiPatientData.POSTCODE;
                patientWithoutPostCode.SetDistanceFromWythenshawe(_context, _logger);
                _context.Update(patientWithoutPostCode);
            }
            _context.SaveChanges();
        
            return RedirectToAction("New", "Imports");
        }

        public IActionResult OnPostAddTask()
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                var guid = Guid.NewGuid().ToString();

                var patientsWithoutPostCodes = _context.Patients; //.Where(p => p.PostCode == null);

                foreach (var patientWithoutPostCode in patientsWithoutPostCodes)
                {
                    var lpiPatientData = _pasDbContext.LpiPatientData.FirstOrDefault(p => p.RM2Number() == patientWithoutPostCode.RM2Number);
                    if (lpiPatientData == null || lpiPatientData.POSTCODE == null) continue;
                    patientWithoutPostCode.PostCode = lpiPatientData.POSTCODE;
                    patientWithoutPostCode.SetDistanceFromWythenshawe(_context, _logger);
                    _context.Update(patientWithoutPostCode);
                    _context.SaveChanges();

                }
                _logger.LogInformation(
                    $"Queued Background Task {guid} is complete.");
            });

            return RedirectToAction("New", "Imports");
        }
    }
}