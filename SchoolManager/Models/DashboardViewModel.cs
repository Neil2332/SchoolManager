using SchoolManager.Data.Entities;

namespace SchoolManager.Models
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }

        public int TotalCourses { get; set; }

        public int TotalTeachers { get; set; }

        public int TotalDisciplines { get; set; }

        public int TotalEnrollments { get; set; }

        public List<Enrollment> RecentEnrollments { get; set; }
            = new List<Enrollment>();
    }
}
