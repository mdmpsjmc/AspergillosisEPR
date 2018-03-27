using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Extensions.Validations
{
    public static class PatientVisitValidations
    {
        public static void CheckVisitDate(this Controller controller, PatientVisit patientVisit)
        {
            if (!string.IsNullOrEmpty(controller.Request.Form["VisitDate"]))
            {
                var dateFromForm = DateTime.ParseExact(controller.Request.Form["VisitDate"], 
                                                            @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                patientVisit.VisitDate = dateFromForm;
            }
            else
            {
                controller.ModelState.AddModelError("visitDate", "Visit date cannot be blank");
            }
        }

        public static void CheckIfAtLeastOneIsSelected(this Controller controller, int selectedItems)
        {
            if (selectedItems == 0)
            {
                controller.ModelState.AddModelError("Base", "You need to select at least one item from the lists below");
            }
        }
    }
}
