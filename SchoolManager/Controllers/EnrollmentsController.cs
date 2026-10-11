using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Entities;
using SchoolManager.Data.Repositories;

namespace SchoolManager.Controllers
{
    [Authorize(Roles = "Admin,Secretaria")]
    public class EnrollmentsController : Controller
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IDisciplineRepository _disciplineRepository;

        public EnrollmentsController(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            IDisciplineRepository disciplineRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _disciplineRepository = disciplineRepository;
        }

        public async Task<IActionResult> Index(string? search)
        {
            IQueryable<Enrollment> query = _enrollmentRepository
                .GetAll()
                .Include(e => e.Student)
                .Include(e => e.Discipline);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(e =>
                    e.Student!.FirstName.Contains(search) ||
                    e.Student.LastName.Contains(search) ||
                    e.Student.StudentNumber.Contains(search) ||
                    e.Discipline!.Name.Contains(search) ||
                    e.Discipline.Code.Contains(search));
            }

            var enrollments = await query
                .OrderByDescending(e => e.EnrollmentDate)
                .ThenBy(e => e.Student!.FirstName)
                .ToListAsync();

            ViewBag.Search = search;

            return View(enrollments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _enrollmentRepository
                .GetAll()
                .Include(e => e.Student)
                    .ThenInclude(s => s!.Course)
                .Include(e => e.Discipline)
                .FirstOrDefaultAsync(e => e.Id == id.Value);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        public async Task<IActionResult> Create()
        {
            await LoadSelectionsAsync();

            return View(new Enrollment
            {
                EnrollmentDate = DateTime.Today,
                Status = "Inscrito"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            enrollment.Status = enrollment.Status?.Trim() ?? string.Empty;

            if (!await _studentRepository.ExistsAsync(
                    s => s.Id == enrollment.StudentId))
            {
                ModelState.AddModelError(
                    nameof(enrollment.StudentId),
                    "O aluno selecionado não existe.");
            }

            if (!await _disciplineRepository.ExistsAsync(
                    d => d.Id == enrollment.DisciplineId))
            {
                ModelState.AddModelError(
                    nameof(enrollment.DisciplineId),
                    "A disciplina selecionada não existe.");
            }

            if (await _enrollmentRepository.ExistsAsync(e =>
                    e.StudentId == enrollment.StudentId &&
                    e.DisciplineId == enrollment.DisciplineId))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Este aluno já está inscrito nesta disciplina.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectionsAsync();
                return View(enrollment);
            }

            try
            {
                await _enrollmentRepository.CreateAsync(enrollment);

                TempData["Success"] = "Inscrição criada.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível criar a inscrição.");

                await LoadSelectionsAsync();

                return View(enrollment);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _enrollmentRepository.GetByIdAsync(id.Value);

            if (enrollment == null)
            {
                return NotFound();
            }

            await LoadSelectionsAsync();

            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            enrollment.Status = enrollment.Status?.Trim() ?? string.Empty;

            if (!await _studentRepository.ExistsAsync(
                    s => s.Id == enrollment.StudentId))
            {
                ModelState.AddModelError(
                    nameof(enrollment.StudentId),
                    "O aluno selecionado não existe.");
            }

            if (!await _disciplineRepository.ExistsAsync(
                    d => d.Id == enrollment.DisciplineId))
            {
                ModelState.AddModelError(
                    nameof(enrollment.DisciplineId),
                    "A disciplina selecionada não existe.");
            }

            if (await _enrollmentRepository.ExistsAsync(e =>
                    e.StudentId == enrollment.StudentId &&
                    e.DisciplineId == enrollment.DisciplineId &&
                    e.Id != enrollment.Id))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Este aluno já está inscrito nesta disciplina.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectionsAsync();
                return View(enrollment);
            }

            try
            {
                await _enrollmentRepository.UpdateAsync(enrollment);

                TempData["Success"] = "Inscrição atualizada.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _enrollmentRepository.ExistsAsync(
                        e => e.Id == enrollment.Id))
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "A inscrição foi alterada por outro processo. Tente novamente.");

                await LoadSelectionsAsync();

                return View(enrollment);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar a inscrição.");

                await LoadSelectionsAsync();

                return View(enrollment);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _enrollmentRepository
                .GetAll()
                .Include(e => e.Student)
                .Include(e => e.Discipline)
                .FirstOrDefaultAsync(e => e.Id == id.Value);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            try
            {
                await _enrollmentRepository.DeleteAsync(enrollment);

                TempData["Success"] = "Inscrição eliminada.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Não foi possível eliminar a inscrição.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSelectionsAsync()
        {
            ViewBag.Students = await _studentRepository
                .GetAll()
                .Where(s => s.IsActive)
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync();

            ViewBag.Disciplines = await _disciplineRepository
                .GetAll()
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}
