using System;
using System.Linq;
using RabbitProducersStandard.Data;


namespace RabbitProducersStandard.SGRQ
{
    public class SGRQLastInsertedId
    {
        private AspEPRContext _context;

        public SGRQLastInsertedId()
        {
            _context = new AspEPRContext();
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
