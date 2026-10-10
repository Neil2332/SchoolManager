using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public class CourseRepository 
        : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(DataContext context)
            : base(context)
        {
        }
    }
}
