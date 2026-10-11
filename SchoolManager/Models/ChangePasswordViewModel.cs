using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Palavra-passe atual")]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nova palavra-passe")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "As palavras-passe não coincidem.")]
        [Display(Name = "Confirmar nova palavra-passe")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
