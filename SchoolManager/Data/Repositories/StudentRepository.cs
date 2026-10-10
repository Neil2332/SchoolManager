using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public class StudentRepository
        : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(DataContext context)
            : base(context)
        {
        }
    }
}
