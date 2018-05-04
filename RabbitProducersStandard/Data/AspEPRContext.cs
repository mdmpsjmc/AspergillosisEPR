using RabbitProducersStandard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitProducersStandard.Data
{
    class AspEPRContext : DbContext
    {

        public AspEPRContext() : base("AspEPRContext")
        {
        }

        public DbSet<PatientSTGQuestionnaire> PatientSTGQuestionnaires { get; set; }
    }
}
