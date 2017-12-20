using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class DbImport
    {
        public int ID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ImportedDate { get; set; }
        public string  ImportedFileName { get; set; }
        public int PatientsCount { get; set; }
        public int DbImportTypeId { get; set;  }        
    }
}
