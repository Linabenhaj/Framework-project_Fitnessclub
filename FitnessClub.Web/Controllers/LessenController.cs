using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace FitnessClub.Web.Controllers
{
    [Authorize]
    public class LessenController : Controller
    {
        private readonly FitnessClubDbContext _context;
        private readonly UserManager<Gebruiker> _userManager;

        public LessenController(FitnessClubDbContext context, UserManager<Gebruiker> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lessen
        public async Task<IActionResult> Index(string sortOrder, string currentFilter,
            string searchString, int? pageNumber, string filterTrainer = "all")
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NaamSortParm"] = string.IsNullOrEmpty(sortOrder) ? "naam_desc" : "";
            ViewData["StartTijdSortParm"] = sortOrder == "StartTijd" ? "start_desc" : "StartTijd";
            ViewData["TrainerSortParm"] = sortOrder == "Trainer" ? "trainer_desc" : "Trainer";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentTrainerFilter"] = filterTrainer;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var lessen = from l in _context.Lessen
                         where !l.IsVerwijderd && l.IsActief
                         select l;

            // Filter op trainer
            if (!string.IsNullOrEmpty(filterTrainer) && filterTrainer != "all")
            {
                lessen = lessen.Where(l => l.Trainer == filterTrainer);
            }

            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                lessen = lessen.Where(l => l.Naam.Contains(searchString)
                    || l.Beschrijving.Contains(searchString)
                    || l.Trainer.Contains(searchString)
                    || l.Locatie.Contains(searchString));
            }

            // Sorting
            lessen = sortOrder switch
            {
                "naam_desc" => lessen.OrderByDescending(l => l.Naam),
                "StartTijd" => lessen.OrderBy(l => l.StartTijd),
                "start_desc" => lessen.OrderByDescending(l => l.StartTijd),
                "Trainer" => lessen.OrderBy(l => l.Trainer),
                "trainer_desc" => lessen.OrderByDescending(l => l.Trainer),
                _ => lessen.OrderBy(l => l.StartTijd)
            };

            // Get unique trainers for filter dropdown
            var trainers = await _context.Lessen
                .Where(l => !l.IsVerwijderd && l.IsActief)
                .Select(l => l.Trainer)
                .Distinct()
                .ToListAsync();

            ViewData["Trainers"] = trainers;

            // Pagination
            int pageSize = 5;
            var paginatedLessen = await PaginatedList<Les>.CreateAsync(lessen.AsNoTracking(), pageNumber ?? 1, pageSize);

            // AJAX check
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_LessenTable", paginatedLessen);
            }

            return View(paginatedLessen);
        }

        // AJAX: Load lessons partial
        [HttpGet]
        public async Task<IActionResult> LoadLessenPartial(string sortOrder, string searchString, string filterTrainer, int? pageNumber)
        {
            var lessen = from l in _context.Lessen
                         where !l.IsVerwijderd && l.IsActief
                         select l;

            if (!string.IsNullOrEmpty(filterTrainer) && filterTrainer != "all")
            {
                lessen = lessen.Where(l => l.Trainer == filterTrainer);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                lessen = lessen.Where(l => l.Naam.Contains(searchString)
                    || l.Trainer.Contains(searchString)
                    || l.Locatie.Contains(searchString));
            }

            lessen = sortOrder switch
            {
                "naam_desc" => lessen.OrderByDescending(l => l.Naam),
                "StartTijd" => lessen.OrderBy(l => l.StartTijd),
                "start_desc" => lessen.OrderByDescending(l => l.StartTijd),
                "Trainer" => lessen.OrderBy(l => l.Trainer),
                "trainer_desc" => lessen.OrderByDescending(l => l.Trainer),
                _ => lessen.OrderBy(l => l.StartTijd)
            };

            int pageSize = 5;
            var paginatedLessen = await PaginatedList<Les>.CreateAsync(lessen.AsNoTracking(), pageNumber ?? 1, pageSize);

            return PartialView("_LessenTable", paginatedLessen);
        }

        // AJAX: Get lesson details
        [HttpGet]
        public async Task<IActionResult> GetLessonDetails(int id)
        {
            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsVerwijderd);

            if (les == null)
            {
                return Json(new { success = false, message = "Les niet gevonden" });
            }

            var availableSpots = les.MaxDeelnemers - les.Inschrijvingen.Count(i => i.Status == "Actief");

            return Json(new
            {
                success = true,
                les = new
                {
                    les.Id,
                    les.Naam,
                    les.Beschrijving,
                    StartTijd = les.StartTijd.ToString("dd/MM/yyyy HH:mm"),
                    EindTijd = les.EindTijd.ToString("dd/MM/yyyy HH:mm"),
                    les.Locatie,
                    les.Trainer,
                    les.MaxDeelnemers,
                    AvailableSpots = availableSpots,
                    IsFull = availableSpots <= 0,
                    Duration = (les.EindTijd - les.StartTijd).TotalMinutes
                }
            });
        }

        // AJAX: Register for lesson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterForLesson(int lessonId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Niet ingelogd" });
            }

            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsVerwijderd);

            if (les == null)
            {
                return Json(new { success = false, message = "Les niet gevonden" });
            }

            // Check if already registered
            var existingRegistration = await _context.Inschrijvingen
                .FirstOrDefaultAsync(i => i.GebruikerId == userId && i.LesId == lessonId && i.Status == "Actief");

            if (existingRegistration != null)
            {
                return Json(new { success = false, message = "Je bent al ingeschreven voor deze les" });
            }

            // Check available spots
            var registeredCount = les.Inschrijvingen.Count(i => i.Status == "Actief");
            if (registeredCount >= les.MaxDeelnemers)
            {
                return Json(new { success = false, message = "Deze les is vol" });
            }

            // Create registration
            var inschrijving = new Inschrijving
            {
                GebruikerId = userId,
                LesId = lessonId,
                InschrijfDatum = DateTime.UtcNow,
                Status = "Actief",
                AangemaaktOp = DateTime.UtcNow
            };

            _context.Inschrijvingen.Add(inschrijving);
            await _context.SaveChangesAsync();

            // Return updated available spots
            var newAvailableSpots = les.MaxDeelnemers - (registeredCount + 1);

            return Json(new
            {
                success = true,
                message = "Succesvol ingeschreven!",
                registrationId = inschrijving.Id,
                availableSpots = newAvailableSpots,
                isFull = newAvailableSpots <= 0
            });
        }

        // GET: Lessen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                    .ThenInclude(i => i.Gebruiker)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsVerwijderd);

            if (les == null)
            {
                return NotFound();
            }

            // Check if current user is registered
            var userId = _userManager.GetUserId(User);
            ViewData["IsRegistered"] = await _context.Inschrijvingen
                .AnyAsync(i => i.GebruikerId == userId && i.LesId == id && i.Status == "Actief");

            return View(les);
        }

        // GET: Lessen/Create
        [Authorize(Roles = "Admin,Trainer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lessen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> Create([Bind("Naam,Beschrijving,StartTijd,EindTijd,MaxDeelnemers,Locatie,Trainer,IsActief")] Les les)
        {
            if (ModelState.IsValid)
            {
                les.AangemaaktOp = DateTime.UtcNow;
                _context.Add(les);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Les succesvol aangemaakt!";
                return RedirectToAction(nameof(Index));
            }
            return View(les);
        }

        // GET: Lessen/Edit/5
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var les = await _context.Lessen.FindAsync(id);
            if (les == null || les.IsVerwijderd)
            {
                return NotFound();
            }
            return View(les);
        }

        // POST: Lessen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naam,Beschrijving,StartTijd,EindTijd,MaxDeelnemers,Locatie,Trainer,IsActief,AangemaaktOp")] Les les)
        {
            if (id != les.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    les.GewijzigdOp = DateTime.UtcNow;
                    _context.Update(les);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Les succesvol bijgewerkt!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LesExists(les.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(les);
        }

        // GET: Lessen/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var les = await _context.Lessen
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsVerwijderd);

            if (les == null)
            {
                return NotFound();
            }

            return View(les);
        }

        // POST: Lessen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var les = await _context.Lessen.FindAsync(id);
            if (les != null)
            {
                les.IsVerwijderd = true;
                les.VerwijderdOp = DateTime.UtcNow;
                _context.Lessen.Update(les);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Les succesvol verwijderd!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LesExists(int id)
        {
            return _context.Lessen.Any(e => e.Id == id && !e.IsVerwijderd);
        }
    }
}