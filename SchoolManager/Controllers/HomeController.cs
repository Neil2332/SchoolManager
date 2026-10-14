using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Repositories;
using SchoolManager.Models;
using System.Diagnostics;

namespace SchoolManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public HomeController(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            IDisciplineRepository disciplineRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _disciplineRepository = disciplineRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalStudents = await _studentRepository
                    .GetAll()
                    .CountAsync(),

                TotalCourses = await _courseRepository
                    .GetAll()
                    .CountAsync(),

                TotalTeachers = await _teacherRepository
                    .GetAll()
                    .CountAsync(),

                TotalDisciplines = await _disciplineRepository
                    .GetAll()
                    .CountAsync(),

                TotalEnrollments = await _enrollmentRepository
                    .GetAll()
                    .CountAsync(),

                RecentEnrollments = await _enrollmentRepository
                    .GetAll()
                    .Include(e => e.Student)
                    .Include(e => e.Discipline)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .ThenByDescending(e => e.Id)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ??
                            HttpContext.TraceIdentifier
            });
        }
    }
}