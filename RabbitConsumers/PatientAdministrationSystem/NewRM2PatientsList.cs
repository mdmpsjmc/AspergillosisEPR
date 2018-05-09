using AspergillosisEPR.Data;
using RabbitConsumers.DbFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitConsumers.PatientAdministrationSystem
{
    class NewRM2PatientsList
    {
        private AspergillosisContext _context;

        public NewRM2PatientsList()
        {
            _context = new AspergillosisContextFactory().CreateDbContext();
        }

        public List<string> GetNewRM2Numbers()
        {
           return  _context.TemporaryNewPatient
                           .Where(p => !string.IsNullOrEmpty(p.RM2Number) && p.ImportedAsRealPatient == false)
                           .ToList()
                           .Select(p => p.RM2Number.Replace("RM2", String.Empty).Trim())
                           .ToList();
        }

        public AspergillosisContext Context()
        {
            return _context;
        }
            
    }
}
