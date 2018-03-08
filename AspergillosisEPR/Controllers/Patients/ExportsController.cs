using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Lib;
using static AspergillosisEPR.Services.ViewToString;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace AspergillosisEPR.Controllers.Patients
{
    [Route("patients/{id:int}/exports/excel")]
    public class ExportsController : Controller
    {
        protected PatientManager _patientManager;
        protected AspergillosisContext _context;
        protected string _fileStoragePath;
        protected IHostingEnvironment _hostingEnvironment;

        public ExportsController(AspergillosisContext context, IHostingEnvironment hostingEnvironment) 
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _patientManager = new PatientManager(context, Request);
        }

        protected async Task<PatientDetailsViewModel> GetExportViewModel(int id)
        {
            var patient = await _patientManager.FindPatientWithRelationsByIdAsync(id);
            var patientDetailsViewModel = PatientDetailsViewModel.BuildPatientViewModel(_context, patient);
            SetItemsToShowInExport(patientDetailsViewModel);
            return patientDetailsViewModel;
        }

        protected void SetItemsToShowInExport(PatientDetailsViewModel patientDetailsViewModel)
        {
            var displayControlKeys = Request.Form.Keys.Where(k => k.Contains("Show")).ToList();

            foreach (var key in DetailsDisplayControlProperties())
            {
                var displayKeyValue = Request.Form[key.Name];
                var propertyInfo = patientDetailsViewModel.GetType().GetProperty(key.Name);
                if (displayKeyValue == "on")
                {
                    propertyInfo.SetValue(patientDetailsViewModel, true);                    
                }
                else
                {
                    propertyInfo.SetValue(patientDetailsViewModel, false);
                }
            }
        }

        protected List<PropertyInfo> DetailsDisplayControlProperties()
        {
            return typeof(PatientDetailsViewModel).GetProperties().
                                                   Where(p => p.Name.ToString().Contains("Show")).
                                                   ToList();
        }

        protected FileContentResult GetFileContentResult(byte[] outputFileBytes, string fileExtension, string contentType)
        {
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
            using (FileStream stream = new FileStream(_fileStoragePath + DateTime.UtcNow.Ticks.ToString() + fileExtension, FileMode.Create))
            {
                stream.Write(outputFileBytes, 0, outputFileBytes.Length);
            }
            var fileContentResult = new FileContentResult(outputFileBytes, contentType);
            return fileContentResult;
        }
    }
}