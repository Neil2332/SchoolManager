using System.ComponentModel.DataAnnotations;

namespace SchoolManager.Data.Entities
{
    public class Enrollment : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Aluno")]
        public int StudentId { get; set; }

        public Student? Student { get; set; }

        [Display(Name = "Disciplina")]
        public int DisciplineId { get; set; }

        public Discipline? Discipline { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data da inscrição")]
        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(30)]
        [Display(Name = "Estado")]
        public string Status { get; set; } = "Inscrito";

        [Range(0, 20)]
        [Display(Name = "Nota final")]
        public decimal? FinalGrade { get; set; }
    }
}
