using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Models
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(80)]
        [Display(Name = "Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        [Display(Name = "Apelido")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Palavra-passe")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "As palavras-passe não coincidem.")]
        [Display(Name = "Confirmar palavra-passe")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
