using System;
using System.Linq;
using RabbitProducersStandard.Data;


namespace RabbitProducersStandard.SGRQ
{
    public class SGRQLastInsertedId
    {
        public static AspEPRContext Context;

        public SGRQLastInsertedId()
        {
            Context = new AspEPRContext();
        }

        public string Get()
        {
            var sgrq =  Context.PatientSTGQuestionnaires
                               .Where(q => q.OriginalImportedId != null)
                               .ToList()
                               .OrderByDescending(q => Int32.Parse(q.OriginalImportedId))
                               .FirstOrDefault();
            if (sgrq == null) return null;
            return sgrq.OriginalImportedId;
        }

    }
}
