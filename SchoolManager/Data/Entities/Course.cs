using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Data.Entities
{
    public class Course : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Código")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Curso")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Range(1, 12)]
        [Display(Name = "Semestres")]
        public int Semesters { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; } = true;

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<CourseDiscipline> CourseDisciplines { get; set; } =
            new List<CourseDiscipline>();
    }
}

