using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public interface IStudentRepository
    {
        public interface IStudentRepository : IGenericRepository<Student>
        {
        }
    }
}
