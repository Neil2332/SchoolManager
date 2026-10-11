using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
