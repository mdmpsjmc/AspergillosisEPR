using AspergillosisEPR.Lib;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientDataTableViewModel  : DTCollection<PatientDataTableViewModel>    {
        public int ID {get; set;}
        public string PrimaryDiagnosis { get; set; }
        public string RM2Number { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DOB { get; set; }

    }
}
