using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Models
{
    public class ProfileViewModel
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
    }
}
