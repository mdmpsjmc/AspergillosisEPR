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
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string LoginName { get; set; }

        public List<string> RoleIds(ApplicationDbContext context)
        {
            return context.UserRoles.Where(u => u.UserId == Id).Select(ur => ur.RoleId).ToList();
        }

        public static string GenerateUsername(string firstName, string lastName)
        {
            return (firstName[0] + lastName).ToLower();
        }


    }
}
