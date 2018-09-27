using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using AspergillosisEPR.Lib.Importers.ClinicLetters;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Hosting;

namespace AspergillosisEPR.Controllers
{
 
    public class ClinicLettersBatchImportController : Controller
    {
        private readonly IFileProvider _fileProvider;
        private readonly IConfiguration _configuration;
        private readonly string _dataDirectory;
        private readonly AspergillosisContext _context;
        private readonly PASDbContext _pasContext;
        private readonly IHostingEnvironment _environment;
        private BatchClinicLettersImporter _importer;

        public ClinicLettersBatchImportController(IFileProvider fileProvider, 
                                                  AspergillosisContext context, 
                                                  PASDbContext pasContext,
                                                  IHostingEnvironment hostingEnvironment,
                                                  IConfiguration configuration)
        {
            _fileProvider = fileProvider;            
            _configuration = configuration;
            _context = context;
            _pasContext = pasContext;
            _environment = hostingEnvironment;
            _dataDirectory = _configuration.GetSection("clinicLettersDirectory").Value.ToString();
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        public IActionResult Create()
        {                       
            var contents = _fileProvider.GetDirectoryContents(_dataDirectory);
            var files = contents.Where(file => file.Name.Contains(".docx") || file.Name.Contains(".doc")).ToList();
            _importer = new BatchClinicLettersImporter(files, _context, _pasContext);
            _importer.Import();
            return View(files);
        }

        [HttpPost, ActionName("Update")]
        public IActionResult Update()
        {
            var importer = new PdfContentImporter(_configuration, _context, _environment, false);
            importer.Run();
            return Json(new { result = importer.Imported });
        }
    }
}