using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Data.Entities
{
    public class Teacher : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "N.º de funcionário")]
        public string EmployeeNumber { get; set; } = string.Empty;

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
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Especialidade")]
        public string Specialization { get; set; } = string.Empty;

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; } = true;

        public ICollection<Discipline> Disciplines { get; set; } =
            new List<Discipline>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
