using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Entities;
using SchoolManager.Data.Repositories;

namespace SchoolManager.Controllers
{
    [Authorize(Roles = "Admin,Secretaria")]
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string? search)
        {
            IQueryable<Course> query = _courseRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(c =>
                    c.Code.Contains(search) ||
                    c.Name.Contains(search));
            }

            var courses = await query
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Search = search;

            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepository
                .GetAll()
                .Include(c => c.Students)
                .Include(c => c.CourseDisciplines)
                    .ThenInclude(cd => cd.Discipline)
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View(new Course
            {
                IsActive = true,
                Semesters = 1
            });
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            course.Code = course.Code?.Trim() ?? string.Empty;
            course.Name = course.Name?.Trim() ?? string.Empty;
            course.Description = course.Description?.Trim();

            if (await _courseRepository.ExistsAsync(
                    c => c.Code == course.Code))
            {
                ModelState.AddModelError(
                    nameof(course.Code),
                    "Já existe um curso com este código.");
            }

            if (!ModelState.IsValid)
            {
                return View(course);
            }

            try
            {
                await _courseRepository.CreateAsync(course);

                TempData["Success"] = "Curso criado.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível criar o curso.");

                return View(course);
            }
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepository.GetByIdAsync(id.Value);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            course.Code = course.Code?.Trim() ?? string.Empty;
            course.Name = course.Name?.Trim() ?? string.Empty;
            course.Description = course.Description?.Trim();

            if (await _courseRepository.ExistsAsync(
                    c => c.Code == course.Code &&
                         c.Id != course.Id))
            {
                ModelState.AddModelError(
                    nameof(course.Code),
                    "Já existe outro curso com este código.");
            }

            if (!ModelState.IsValid)
            {
                return View(course);
            }

            try
            {
                await _courseRepository.UpdateAsync(course);

                TempData["Success"] = "Curso atualizado.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _courseRepository.ExistsAsync(
                        c => c.Id == course.Id))
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "O curso foi alterado por outro processo. Tente novamente.");

                return View(course);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar o curso.");

                return View(course);
            }
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepository
                .GetAll()
                .Include(c => c.Students)
                .Include(c => c.CourseDisciplines)
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseRepository
                .GetAll()
                .Include(c => c.Students)
                .Include(c => c.CourseDisciplines)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            if (course.Students.Any())
            {
                TempData["Error"] =
                    "Não é possível eliminar este curso porque possui alunos associados.";

                return RedirectToAction(nameof(Index));
            }

            if (course.CourseDisciplines.Any())
            {
                TempData["Error"] =
                    "Não é possível eliminar este curso porque possui disciplinas no plano curricular.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _courseRepository.DeleteAsync(course);

                TempData["Success"] = "Curso eliminado.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Não foi possível eliminar o curso porque existem dados relacionados.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
