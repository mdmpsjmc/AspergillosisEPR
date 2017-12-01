using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AspergillosisEPR.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        [Required]
        [Display(Name = "Last Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string LastName { get; set; }

        public static string GenerateUsername(string firstName, string lastName)
        {
            return (firstName[0] + lastName).ToLower();
        }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

    }
}
