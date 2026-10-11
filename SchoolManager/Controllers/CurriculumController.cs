using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Entities;
using SchoolManager.Data.Repositories;

namespace SchoolManager.Controllers
{
    [Authorize(Roles = "Admin,Secretaria")]
    public class CurriculumController : Controller
    {
        private readonly ICourseDisciplineRepository _courseDisciplineRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IDisciplineRepository _disciplineRepository;

        public CurriculumController(
            ICourseDisciplineRepository courseDisciplineRepository,
            ICourseRepository courseRepository,
            IDisciplineRepository disciplineRepository)
        {
            _courseDisciplineRepository = courseDisciplineRepository;
            _courseRepository = courseRepository;
            _disciplineRepository = disciplineRepository;
        }

        public async Task<IActionResult> Index(int? courseId)
        {
            IQueryable<CourseDiscipline> query = _courseDisciplineRepository
                .GetAll()
                .Include(cd => cd.Course)
                .Include(cd => cd.Discipline);

            if (courseId.HasValue)
            {
                query = query.Where(cd => cd.CourseId == courseId.Value);
            }

            ViewBag.CourseId = courseId;
            ViewBag.Courses = await _courseRepository
                .GetAll()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var items = await query
                .OrderBy(cd => cd.Course!.Name)
                .ThenBy(cd => cd.Semester)
                .ThenBy(cd => cd.Discipline!.Name)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            await LoadSelectionsAsync();
            return View(new CourseDiscipline
            {
                Semester = 1
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseDiscipline model)
        {
            if (!await _courseRepository.ExistsAsync(c => c.Id == model.CourseId))
            {
                ModelState.AddModelError(
                    nameof(model.CourseId),
                    "O curso selecionado não existe.");
            }

            if (!await _disciplineRepository.ExistsAsync(d => d.Id == model.DisciplineId))
            {
                ModelState.AddModelError(
                    nameof(model.DisciplineId),
                    "A disciplina selecionada não existe.");
            }

            if (await _courseDisciplineRepository.ExistsAsync(cd =>
                    cd.CourseId == model.CourseId &&
                    cd.DisciplineId == model.DisciplineId))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Esta disciplina já está associada a este curso.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectionsAsync();
                return View(model);
            }

            try
            {
                await _courseDisciplineRepository.CreateAsync(model);

                TempData["Success"] =
                    "Disciplina adicionada ao plano curricular.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível adicionar a disciplina ao plano curricular.");

                await LoadSelectionsAsync();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _courseDisciplineRepository.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            try
            {
                await _courseDisciplineRepository.DeleteAsync(item);

                TempData["Success"] =
                    "Disciplina removida do plano curricular.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Não foi possível remover a disciplina do plano curricular.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSelectionsAsync()
        {
            ViewBag.Courses = await _courseRepository
                .GetAll()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Disciplines = await _disciplineRepository
                .GetAll()
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}
