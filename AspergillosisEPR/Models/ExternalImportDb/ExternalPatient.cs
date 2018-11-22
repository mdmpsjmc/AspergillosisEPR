using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.ExternalImportDb
{
    [Table("Patients")]
    public class ExternalPatient
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string RM2Number { get; set; }
        public int? PatientStatusId { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string NhsNumber { get; set; }
        public string GenericNote { get; set; }
        public string PostCode { get; set; }
        public double DistanceFromWythenshawe { get; set; }

        public static ExternalPatient BuildFromPatient(Patient patient)
        {
            var externalPatient = new ExternalPatient();
            externalPatient.LastName = patient.LastName;
            externalPatient.FirstName = patient.FirstName;
            externalPatient.Gender = patient.Gender;
            externalPatient.DOB = patient.DOB;
            externalPatient.RM2Number = "RM2" + patient.RM2Number;
            externalPatient.PatientStatusId = patient.PatientStatusId;
            externalPatient.DateOfDeath = patient.DateOfDeath;
            externalPatient.NhsNumber = patient.NhsNumber;
            externalPatient.GenericNote = patient.GenericNote;
            externalPatient.PostCode = patient.PostCode;
            externalPatient.DistanceFromWythenshawe = patient.DistanceFromWythenshawe;
            return externalPatient;
        }
    }
}
