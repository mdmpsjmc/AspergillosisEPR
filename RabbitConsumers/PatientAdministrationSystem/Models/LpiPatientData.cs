using AspergillosisEPR.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RabbitConsumers.PatientAdministrationSystem.Models
{
    class LpiPatientData
    {
        public string FACIL_ID { get; set; }
        public string SURNAME { get; set; }
        public string GIVEN_NAME { get; set; }
        public string SECOND_NAME { get; set; }
        public string SEX { get; set; }
        public string NHS_NUMBER { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyyMMdd}")]
        public string DEATH_TIME { get; set; }
        public string DEATH_INDICATOR { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyyMMdd}")]
        public DateTime DOB { get; set; }
        public string ROWID { get; set; }

        public string RM2Number()
        {
            return FACIL_ID.ToString()
                           .Replace("RM2", String.Empty)
                           .Replace("rm2", String.Empty)
                           .Trim();
        }

        public string FirstName()
        {
            return SURNAME + " " + SECOND_NAME;
        }

        public string Gender()
        {
            if (SEX == "F")
            {
                return "Female";
            }
            if (SEX == "M")
            {
                return "Male";
            }
            return null;
        }

        public int PatientStatusId(AspergillosisContext context, int deadStatus, int aliveStatus)
        {
           if (string.IsNullOrEmpty(DEATH_INDICATOR) || DEATH_INDICATOR == "Y")
           {
                return aliveStatus;
           }
           if (DEATH_INDICATOR == "Y")
            {
                return deadStatus;
            }
            return 0;
        }
    }
}
