using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Data.Entities
{
    public class Discipline : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Código")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        [Display(Name = "Disciplina")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 1000)]
        [Display(Name = "Carga horária")]
        public int WorkloadHours { get; set; }

        [Display(Name = "Professor")]
        public int? TeacherId { get; set; }

        public Teacher? Teacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } =
            new List<Enrollment>();

        public ICollection<CourseDiscipline> CourseDisciplines { get; set; } =
            new List<CourseDiscipline>();
    }
}
