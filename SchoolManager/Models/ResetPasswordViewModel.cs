using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nova palavra-passe")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "As palavras-passe não coincidem.")]
        [Display(Name = "Confirmar palavra-passe")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
