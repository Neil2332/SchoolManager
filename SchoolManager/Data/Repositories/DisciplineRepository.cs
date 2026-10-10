using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public class DisciplineRepository
        : GenericRepository<Discipline>, IDisciplineRepository
    {
        public DisciplineRepository(DataContext context)
            : base(context)
        {
        }
    }
}
