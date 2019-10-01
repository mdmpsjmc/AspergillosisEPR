using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers.Patients
{
    public class SmokingStatusesController : PatientBaseController
    {
        private DropdownListsResolver _listResolver;
        public SmokingStatusesController(AspergillosisContext context) : base(context)
        {
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);
        }

    [Route("SmokingStatuses/Edit")]
    [HttpGet]
    public IActionResult Edit(int patientId)
    {
      var patient = _context.Patients
                    .Where(p => p.ID == patientId)
                    .Include(p => p.PatientSmokingDrinkingStatus)
                    .FirstOrDefault();
      PatientSmokingDrinkingStatus smokingStatus;
      smokingStatus = patient.PatientSmokingDrinkingStatus;
    
      if (smokingStatus == null)
      {
        smokingStatus = new PatientSmokingDrinkingStatus()
        {
          PatientId = patient.ID
        };
        ViewBag.SmokingStatusId = _listResolver.PopulateSmokingStatusesDropdownList();
      }
      else
      {
        ViewBag.SmokingStatusId = _listResolver.PopulateSmokingStatusesDropdownList(smokingStatus.SmokingStatusId);
      }
      return PartialView("~/Views/Patients/SmokingStatuses/_Edit.cshtml", smokingStatus);
    }

        [Route("SmokingDrinking")]
        public IActionResult Details(int patientId)
        {
            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientSmokingDrinkingStatus)
                                    .ThenInclude(ss => ss.SmokingStatus)
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/SmokingStatuses/_Details.cshtml", patient);
        }
  }
}