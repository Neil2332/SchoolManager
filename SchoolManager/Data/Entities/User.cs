using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Data.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(80)]
        [Display(Name = "Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        [Display(Name = "Apelido")]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";
    }
}
