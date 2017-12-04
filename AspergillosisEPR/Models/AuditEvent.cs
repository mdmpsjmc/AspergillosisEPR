using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class AuditEvent
    {
        public int ID { get; set; }
        public DateTime InsertedDate
        {
            get
            {
                return dateCreated.HasValue
                   ? dateCreated.Value
                   : DateTime.Now;
            }

            set { dateCreated = value; }
        }
        private DateTime? dateCreated = null;
        public DateTime? LastUpdatedDate { get; set; }
        public string Data { get; set; }
    }
}
