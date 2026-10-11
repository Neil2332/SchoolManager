using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Entities;
using SchoolManager.Data.Repositories;

namespace SchoolManager.Controllers
{
    [Authorize(Roles = "Admin,Secretaria")]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;

        public StudentsController(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IActionResult> Index(string? search, int? courseId)
        {
            IQueryable<Student> query = _studentRepository
                .GetAll()
                .Include(s => s.Course);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(s =>
                    s.FirstName.Contains(search) ||
                    s.LastName.Contains(search) ||
                    s.StudentNumber.Contains(search) ||
                    s.Email.Contains(search));
            }

            if (courseId.HasValue)
            {
                query = query.Where(s => s.CourseId == courseId.Value);
            }

            ViewBag.Search = search;
            ViewBag.CourseId = courseId;
            ViewBag.Courses = await _courseRepository
                .GetAll()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var students = await query
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync();

            return View(students);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentRepository
                .GetAll()
                .Include(s => s.Course)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Discipline)
                .FirstOrDefaultAsync(s => s.Id == id.Value);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCoursesAsync();

            return View(new Student
            {
                IsActive = true,
                BirthDate = DateTime.Today.AddYears(-18)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            student.StudentNumber = student.StudentNumber?.Trim() ?? string.Empty;
            student.FirstName = student.FirstName?.Trim() ?? string.Empty;
            student.LastName = student.LastName?.Trim() ?? string.Empty;
            student.Email = student.Email?.Trim() ?? string.Empty;

            if (await _studentRepository.ExistsAsync(
                    s => s.StudentNumber == student.StudentNumber))
            {
                ModelState.AddModelError(
                    nameof(student.StudentNumber),
                    "Já existe um aluno com este número.");
            }

            if (await _studentRepository.ExistsAsync(
                    s => s.Email == student.Email))
            {
                ModelState.AddModelError(
                    nameof(student.Email),
                    "Já existe um aluno com este email.");
            }

            if (!await _courseRepository.ExistsAsync(
                    c => c.Id == student.CourseId))
            {
                ModelState.AddModelError(
                    nameof(student.CourseId),
                    "O curso selecionado não existe.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCoursesAsync();
                return View(student);
            }

            try
            {
                await _studentRepository.CreateAsync(student);

                TempData["Success"] = "Aluno criado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível criar o aluno.");

                await LoadCoursesAsync();
                return View(student);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetByIdAsync(id.Value);

            if (student == null)
            {
                return NotFound();
            }

            await LoadCoursesAsync();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            student.StudentNumber = student.StudentNumber?.Trim() ?? string.Empty;
            student.FirstName = student.FirstName?.Trim() ?? string.Empty;
            student.LastName = student.LastName?.Trim() ?? string.Empty;
            student.Email = student.Email?.Trim() ?? string.Empty;

            if (await _studentRepository.ExistsAsync(
                    s => s.StudentNumber == student.StudentNumber &&
                         s.Id != student.Id))
            {
                ModelState.AddModelError(
                    nameof(student.StudentNumber),
                    "Já existe outro aluno com este número.");
            }

            if (await _studentRepository.ExistsAsync(
                    s => s.Email == student.Email &&
                         s.Id != student.Id))
            {
                ModelState.AddModelError(
                    nameof(student.Email),
                    "Já existe outro aluno com este email.");
            }

            if (!await _courseRepository.ExistsAsync(
                    c => c.Id == student.CourseId))
            {
                ModelState.AddModelError(
                    nameof(student.CourseId),
                    "O curso selecionado não existe.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCoursesAsync();
                return View(student);
            }

            try
            {
                await _studentRepository.UpdateAsync(student);

                TempData["Success"] = "Aluno atualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _studentRepository.ExistsAsync(
                        s => s.Id == student.Id))
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "O aluno foi alterado por outro processo. Tente novamente.");

                await LoadCoursesAsync();
                return View(student);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar o aluno.");

                await LoadCoursesAsync();
                return View(student);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentRepository
                .GetAll()
                .Include(s => s.Course)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id.Value);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentRepository
                .GetAll()
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            if (student.Enrollments.Any())
            {
                TempData["Error"] =
                    "Não é possível eliminar este aluno porque possui inscrições.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _studentRepository.DeleteAsync(student);

                TempData["Success"] = "Aluno eliminado.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Não foi possível eliminar o aluno porque existem dados relacionados.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCoursesAsync()
        {
            ViewBag.Courses = await _courseRepository
                .GetAll()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
