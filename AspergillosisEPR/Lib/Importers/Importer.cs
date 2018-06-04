using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers
{
    public abstract class Importer
    {
        protected  FileStream _stream;
        protected  IFormFile _file { get; set; }
        protected  AspergillosisContext _context;
        protected  string _fileExtension { get; set; }
        public List<dynamic> Imported { get; set; }
    }
}
