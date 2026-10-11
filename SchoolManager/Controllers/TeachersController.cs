using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Entities;
using SchoolManager.Data.Repositories;

namespace SchoolManager.Controllers
{
    [Authorize(Roles = "Admin,Secretaria")]
    public class TeachersController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeachersController(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<IActionResult> Index(string? search)
        {
            IQueryable<Teacher> query = _teacherRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(t =>
                    t.FirstName.Contains(search) ||
                    t.LastName.Contains(search) ||
                    t.EmployeeNumber.Contains(search) ||
                    t.Specialization.Contains(search));
            }

            var teachers = await query
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.LastName)
                .ToListAsync();

            ViewBag.Search = search;

            return View(teachers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _teacherRepository
                .GetAll()
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == id.Value);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        public IActionResult Create()
        {
            return View(new Teacher
            {
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            teacher.EmployeeNumber = teacher.EmployeeNumber?.Trim() ?? string.Empty;
            teacher.FirstName = teacher.FirstName?.Trim() ?? string.Empty;
            teacher.LastName = teacher.LastName?.Trim() ?? string.Empty;
            teacher.Email = teacher.Email?.Trim() ?? string.Empty;
            teacher.Specialization = teacher.Specialization?.Trim() ?? string.Empty;

            if (await _teacherRepository.ExistsAsync(
                    t => t.EmployeeNumber == teacher.EmployeeNumber))
            {
                ModelState.AddModelError(
                    nameof(teacher.EmployeeNumber),
                    "Já existe um professor com este número de funcionário.");
            }

            if (await _teacherRepository.ExistsAsync(
                    t => t.Email == teacher.Email))
            {
                ModelState.AddModelError(
                    nameof(teacher.Email),
                    "Já existe um professor com este email.");
            }

            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            try
            {
                await _teacherRepository.CreateAsync(teacher);

                TempData["Success"] = "Professor criado.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível criar o professor.");

                return View(teacher);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _teacherRepository.GetByIdAsync(id.Value);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            teacher.EmployeeNumber = teacher.EmployeeNumber?.Trim() ?? string.Empty;
            teacher.FirstName = teacher.FirstName?.Trim() ?? string.Empty;
            teacher.LastName = teacher.LastName?.Trim() ?? string.Empty;
            teacher.Email = teacher.Email?.Trim() ?? string.Empty;
            teacher.Specialization = teacher.Specialization?.Trim() ?? string.Empty;

            if (await _teacherRepository.ExistsAsync(
                    t => t.EmployeeNumber == teacher.EmployeeNumber &&
                         t.Id != teacher.Id))
            {
                ModelState.AddModelError(
                    nameof(teacher.EmployeeNumber),
                    "Já existe outro professor com este número de funcionário.");
            }

            if (await _teacherRepository.ExistsAsync(
                    t => t.Email == teacher.Email &&
                         t.Id != teacher.Id))
            {
                ModelState.AddModelError(
                    nameof(teacher.Email),
                    "Já existe outro professor com este email.");
            }

            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            try
            {
                await _teacherRepository.UpdateAsync(teacher);

                TempData["Success"] = "Professor atualizado.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _teacherRepository.ExistsAsync(
                        t => t.Id == teacher.Id))
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "O professor foi alterado por outro processo. Tente novamente.");

                return View(teacher);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar o professor.");

                return View(teacher);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _teacherRepository
                .GetAll()
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == id.Value);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _teacherRepository
                .GetAll()
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            if (teacher.Disciplines.Any())
            {
                TempData["Error"] =
                    "Não é possível eliminar este professor porque possui disciplinas atribuídas.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _teacherRepository.DeleteAsync(teacher);

                TempData["Success"] = "Professor eliminado.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Não foi possível eliminar o professor porque existem dados relacionados.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
