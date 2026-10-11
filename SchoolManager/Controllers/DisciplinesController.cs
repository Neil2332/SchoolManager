using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Entities;
using SchoolManager.Data.Repositories;

namespace SchoolManager.Controllers
{
    [Authorize(Roles = "Admin,Secretaria")]
    public class DisciplinesController : Controller
    {
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly ITeacherRepository _teacherRepository;

        public DisciplinesController(
            IDisciplineRepository disciplineRepository,
            ITeacherRepository teacherRepository)
        {
            _disciplineRepository = disciplineRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<IActionResult> Index(string? search)
        {
            IQueryable<Discipline> query = _disciplineRepository
                .GetAll()
                .Include(d => d.Teacher);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(d =>
                    d.Code.Contains(search) ||
                    d.Name.Contains(search));
            }

            var disciplines = await query
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.Search = search;

            return View(disciplines);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _disciplineRepository
                .GetAll()
                .Include(d => d.Teacher)
                .Include(d => d.Enrollments)
                    .ThenInclude(e => e.Student)
                .Include(d => d.CourseDisciplines)
                    .ThenInclude(cd => cd.Course)
                .FirstOrDefaultAsync(d => d.Id == id.Value);

            if (discipline == null)
            {
                return NotFound();
            }

            return View(discipline);
        }

        public async Task<IActionResult> Create()
        {
            await LoadTeachersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discipline discipline)
        {
            discipline.Code = discipline.Code?.Trim() ?? string.Empty;
            discipline.Name = discipline.Name?.Trim() ?? string.Empty;

            if (await _disciplineRepository.ExistsAsync(
                    d => d.Code == discipline.Code))
            {
                ModelState.AddModelError(
                    nameof(discipline.Code),
                    "Já existe uma disciplina com este código.");
            }

            if (discipline.TeacherId.HasValue &&
                !await _teacherRepository.ExistsAsync(
                    t => t.Id == discipline.TeacherId.Value))
            {
                ModelState.AddModelError(
                    nameof(discipline.TeacherId),
                    "O professor selecionado não existe.");
            }

            if (!ModelState.IsValid)
            {
                await LoadTeachersAsync();
                return View(discipline);
            }

            try
            {
                await _disciplineRepository.CreateAsync(discipline);

                TempData["Success"] = "Disciplina criada.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível criar a disciplina.");

                await LoadTeachersAsync();
                return View(discipline);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _disciplineRepository.GetByIdAsync(id.Value);

            if (discipline == null)
            {
                return NotFound();
            }

            await LoadTeachersAsync();
            return View(discipline);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Discipline discipline)
        {
            if (id != discipline.Id)
            {
                return NotFound();
            }

            discipline.Code = discipline.Code?.Trim() ?? string.Empty;
            discipline.Name = discipline.Name?.Trim() ?? string.Empty;

            if (await _disciplineRepository.ExistsAsync(
                    d => d.Code == discipline.Code &&
                         d.Id != discipline.Id))
            {
                ModelState.AddModelError(
                    nameof(discipline.Code),
                    "Já existe outra disciplina com este código.");
            }

            if (discipline.TeacherId.HasValue &&
                !await _teacherRepository.ExistsAsync(
                    t => t.Id == discipline.TeacherId.Value))
            {
                ModelState.AddModelError(
                    nameof(discipline.TeacherId),
                    "O professor selecionado não existe.");
            }

            if (!ModelState.IsValid)
            {
                await LoadTeachersAsync();
                return View(discipline);
            }

            try
            {
                await _disciplineRepository.UpdateAsync(discipline);

                TempData["Success"] = "Disciplina atualizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _disciplineRepository.ExistsAsync(
                        d => d.Id == discipline.Id))
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "A disciplina foi alterada por outro processo. Tente novamente.");

                await LoadTeachersAsync();
                return View(discipline);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar a disciplina.");

                await LoadTeachersAsync();
                return View(discipline);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _disciplineRepository
                .GetAll()
                .Include(d => d.Teacher)
                .Include(d => d.Enrollments)
                .Include(d => d.CourseDisciplines)
                .FirstOrDefaultAsync(d => d.Id == id.Value);

            if (discipline == null)
            {
                return NotFound();
            }

            return View(discipline);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discipline = await _disciplineRepository
                .GetAll()
                .Include(d => d.Enrollments)
                .Include(d => d.CourseDisciplines)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discipline == null)
            {
                return NotFound();
            }

            if (discipline.Enrollments.Any())
            {
                TempData["Error"] =
                    "Não é possível eliminar esta disciplina porque possui inscrições.";

                return RedirectToAction(nameof(Index));
            }

            if (discipline.CourseDisciplines.Any())
            {
                TempData["Error"] =
                    "Não é possível eliminar esta disciplina porque está associada a um plano curricular.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _disciplineRepository.DeleteAsync(discipline);

                TempData["Success"] = "Disciplina eliminada.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Não foi possível eliminar a disciplina porque existem dados relacionados.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadTeachersAsync()
        {
            ViewBag.Teachers = await _teacherRepository
                .GetAll()
                .Where(t => t.IsActive)
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.LastName)
                .ToListAsync();
        }
    }
}
