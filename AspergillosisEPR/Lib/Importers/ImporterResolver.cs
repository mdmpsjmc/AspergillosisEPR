using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers
{
    public class ImporterResolver
    {
        private readonly AspergillosisContext _context;

        public ImporterResolver(AspergillosisContext context)
        {
            _context = context;
        }

        public dynamic Resolve(string importerClassName, object[] constructorParams)
        {
            return GetImporterInstance(importerClassName, constructorParams);
        }

        public DbImportType GetDbImporterTypeById(int ID)
        {
            return _context.DBImportTypes.Where(it => it.ID == ID).FirstOrDefault();
        }

        private Importer GetImporterInstance(string importerClassName, object[] constructorParams)
        {
            Type type = GetDbImporterTypeByName(importerClassName);
            return (Importer)Activator.CreateInstance(type, constructorParams);
        }

        private Type GetDbImporterTypeByName(string importerClassName)
        {
            return Type.GetType("AspergillosisEPR.Lib.Importers.Implementations." + importerClassName);
        }
    }
}
