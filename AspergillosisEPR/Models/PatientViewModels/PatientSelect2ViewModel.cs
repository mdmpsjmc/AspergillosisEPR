using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientSelect2ViewModel
    {
        public int id { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl {
            get
            {
                return "/img/avatars/" + Patient.Gender.ToLower() + ".png";    
            }
        }
        public Patient Patient { get; set; }
        public string Description
        {
            get
            {
                return "RM2 Number: " + Patient.RM2Number + ", Age: " + Patient.Age(); 
            }
        }

    }
}
