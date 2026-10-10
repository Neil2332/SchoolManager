using SchoolManager.Data.Entities;

namespace SchoolManager.Data.Repositories
{
    public class CourseDisciplineRepository
        : GenericRepository<CourseDiscipline>,
          ICourseDisciplineRepository
    {
        public CourseDisciplineRepository(DataContext context)
            : base(context)
        {
        }
    }
}
