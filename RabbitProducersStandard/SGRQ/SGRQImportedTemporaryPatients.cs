using RabbitProducersStandard.Data;
using RabbitProducersStandard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitProducersStandard.SGRQ
{
    class SGRQImportedTemporaryPatients
    {
        private AspEPRContext _context;

        public SGRQImportedTemporaryPatients(AspEPRContext context)
        {
            _context = context;
        }

        public List<TemporaryNewPatient> Get()
        {
            return _context.TemporaryNewPatient
                           .Where(p => p.ImportedAsRealPatient == true)
                           .ToList();
        }

    }
}
