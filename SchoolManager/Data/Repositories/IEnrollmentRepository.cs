using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public interface IEnrollmentRepository
    {
        public interface IEnrollmentRepository : IGenericRepository<Enrollment>
        {
        }
    }
}
