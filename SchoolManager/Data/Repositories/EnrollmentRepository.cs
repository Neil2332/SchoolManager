using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public class EnrollmentRepository
        : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(DataContext context)
            : base(context)
        {
        }
    }
}
