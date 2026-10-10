using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public class TeacherRepository
        : GenericRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(DataContext context)
            : base(context)
        {
        }
    }
}
