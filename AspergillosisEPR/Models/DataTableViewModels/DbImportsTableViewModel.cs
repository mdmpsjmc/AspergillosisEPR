using AspergillosisEPR.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.DataTableViewModels
{
    public class DbImportsTableViewModel : DTCollection<DbImportsTableViewModel>
    {
        public int ID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public double ImportedDate { get; set; }
        public string ImportedFileName { get; set; }
        public int PatientsCount { get; set; }
    }
}
