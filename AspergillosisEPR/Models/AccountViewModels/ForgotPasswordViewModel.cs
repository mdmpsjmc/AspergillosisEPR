
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
