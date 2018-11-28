using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.ExternalImportDb
{
    public class PathologyReport
    {
        [Key]    
        public decimal ObservationGUID { get; set; }
        public string RM2Number { get; set; }
        public string orderSet { get; set; }
        public string OrderItemCode { get; set; }
        public string OrderItemDescription { get; set; }
        public string Result { get; set; }
        public string unit { get; set; }
        public string NormalRange { get; set; }
        public string AbnormalFlag { get; set; }
        public DateTime DatePerformed { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
