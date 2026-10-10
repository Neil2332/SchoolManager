using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public interface ITeacherRepository
    {
        public interface ITeacherRepository : IGenericRepository<Teacher>
        {
        }
    }
}
