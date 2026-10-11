using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Palavra-passe")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }
    }
}
