using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string LoginName { get; set; }

        
        public static string GenerateUsername(string firstName, string lastName)
        {
            return (firstName[0] + lastName).ToLower();
        }


    }
}
