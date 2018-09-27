using AspergillosisEPR.Data;
using AspergillosisEPR.Extensions;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    public class PdfContentImporter
    {
        private string _dataDirectory;
        private string _importFiles;
        private readonly AspergillosisContext _context;
        private ClinicLetterPdfFileImporter _importer;
        private IHostingEnvironment _hostingEnv;
        private bool _deleteImported = false;
        public int Imported { get; set; } = 0;

        public PdfContentImporter(IConfiguration configuration,
                                  AspergillosisContext appDbContext,
                                  IHostingEnvironment hostingEnvironment,
                                  bool deleteImportedFile = false)
        {
            _dataDirectory = configuration.GetSection("DataDirectory").Value;
            _importFiles = configuration.GetSection("ImportFiles").Value;
            _context = appDbContext;
            _deleteImported = deleteImportedFile;
            _hostingEnv = hostingEnvironment;
        }

        public void Run()
        {
            foreach (string fileToImportPath in FilesToImport())
            {
                var ms = new MemoryStream(File.ReadAllBytes(fileToImportPath));
                Action<FileStream, string> readAndSaveFileAction = (stream, fullPath) =>
                {
                    _importer = new ClinicLetterPdfFileImporter(stream, fullPath, _context);
                    string fileContent = _importer.ReadFile();
                    var dateExtractor = new ClnicLettersDateExtractor(fileContent);
                    var rm2Number = dateExtractor.ForRM2Number();
                    var datesList = dateExtractor.Dates();
                    var patient = _context.Patients
                                          .Include(p => p.PatientNACDates)
                                          .Where(p => p.RM2Number == rm2Number)
                                          .FirstOrDefault();
                    if (patient == null) return;
                    if (patient.PatientNACDates == null) patient.PatientNACDates = new PatientNACDates();
                    if (patient.PatientNACDates.FirstSeenAtNAC == null) patient.PatientNACDates.FirstSeenAtNAC = dateExtractor.EarliestDate();
                    patient.PatientNACDates.LastObservationPoint = dateExtractor.LatestDate();
                    _context.Update(patient);
                    Imported++;
                };

                FileImporter.Import(fileToImportPath, readAndSaveFileAction);
                if (_deleteImported) File.Delete(fileToImportPath);
            }
        }

        private List<string> FilesToImport()
        {   
            var filesToImport = new List<string>();
            var contents = _dataDirectory.FileTypes(".pdf");
            foreach (var item in contents)
            {
                if (item.Contains(Extension()))
                {
                    filesToImport.Add(item);
                }
            }
            return filesToImport;
        }

        private string Extension()
        {
            return _importFiles.Replace("*.", String.Empty);
        }
    }
}
