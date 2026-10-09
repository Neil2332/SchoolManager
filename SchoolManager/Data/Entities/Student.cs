using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Data.Entities
{
    public class Student : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "N.º de aluno")]
        public string StudentNumber { get; set; } = string.Empty;

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

        [DataType(DataType.Date)]
        [Display(Name = "Data de nascimento")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Curso")]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } =
            new List<Enrollment>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
