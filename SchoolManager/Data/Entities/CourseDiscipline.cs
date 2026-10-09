using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Data.Entities
{
    public class CourseDiscipline : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Curso")]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        [Display(Name = "Disciplina")]
        public int DisciplineId { get; set; }

        public Discipline? Discipline { get; set; }

        [Range(1, 12)]
        [Display(Name = "Semestre")]
        public int Semester { get; set; }
    }
}
