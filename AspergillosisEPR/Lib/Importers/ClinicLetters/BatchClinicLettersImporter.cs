using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.FileProviders;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    public class BatchClinicLettersImporter
    {
        private readonly AspergillosisContext _context;
        private readonly IEnumerable<IFileInfo> _filesToImport;
        private readonly PASDbContext _pasContext;
        private int _imported;
       

        public BatchClinicLettersImporter(ICollection<IFileInfo> filesToImport, 
                                          AspergillosisContext context, 
                                          PASDbContext pasContext)
        {
            _filesToImport = filesToImport;
            _imported = 0;
            _context = context;
            _pasContext = pasContext;
        }

        public int Import()
        {
            foreach (IFileInfo file in _filesToImport)
            {
                string extension = "." + file.Name.Split(".").Last();
                FileStream fileContent = File.Open(file.PhysicalPath, FileMode.Open);
                var formFile = BuildIformFile(file, fileContent);
                var singleFileImporter = new EPRClinicLetterDocxImporter(fileContent, extension, _context, _pasContext);
                singleFileImporter.ReadDOCXFile();
                _imported++;
            }
            return _imported;
        }

        private FormFile BuildIformFile(IFileInfo file, FileStream stream)
        {            
            return new FormFile(stream, 0, stream.Length, file.Name, file.Name);
        }
    }
}
