using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public interface ICourseRepository
    {
        public interface ICourseRepository : IGenericRepository<Course>
        {
        }
    }
}
