using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using RabbitConsumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitConsumers.SGRQ
{
    public class SGRQLastInsertedId
    {
        private AspergillosisContext _context;

        public SGRQLastInsertedId()
        {
            _context = new AspergillosisContextFactory().CreateDbContext();
        }

        public string Get()
        {
            var sgrq = _context.PatientSTGQuestionnaires
                               .Where(q => q.OriginalImportedId != null)
                               .ToList()
                               .OrderByDescending(q => Int32.Parse(q.OriginalImportedId))
                               .FirstOrDefault();
            if (sgrq == null) return null;
            return sgrq.OriginalImportedId;
        }
    }
}
