using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using FitnessClub.Web.Models;  // Referentie naar je PaginatedList
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

            if (!string.IsNullOrEmpty(filterTrainer) && filterTrainer != "all")
            {
                lessen = lessen.Where(l => l.Trainer == filterTrainer);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                lessen = lessen.Where(l => l.Naam.Contains(searchString)
                    || l.Beschrijving.Contains(searchString)
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

            var trainers = await _context.Lessen
                .Where(l => !l.IsVerwijderd && l.IsActief)
                .Select(l => l.Trainer)
                .Distinct()
                .ToListAsync();

            ViewData["Trainers"] = trainers;

            int pageSize = 5;
            var paginatedLessen = await PaginatedList<Les>.CreateAsync(lessen.AsNoTracking(), pageNumber ?? 1, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_LessenTable", paginatedLessen);
            }

            return View(paginatedLessen);
        }

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

        // Trainer geeft zichzelf aan voor een les (vult Trainer-veld in)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> ClaimAsTrainer(int id, string trainerName)
        {
            if (string.IsNullOrWhiteSpace(trainerName))
            {
                TempData["ErrorMessage"] = "Vul je naam in om je aan te geven als trainer.";
                return RedirectToAction(nameof(Index));
            }

            var les = await _context.Lessen.FirstOrDefaultAsync(l => l.Id == id && !l.IsVerwijderd);
            if (les == null)
            {
                TempData["ErrorMessage"] = "Les niet gevonden.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(les.Trainer))
            {
                TempData["ErrorMessage"] = $"Deze les heeft al een trainer ({les.Trainer}).";
                return RedirectToAction(nameof(Index));
            }

            les.Trainer = trainerName.Trim();
            les.GewijzigdOp = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Je bent aangegeven als trainer voor '{les.Naam}'.";
            return RedirectToAction(nameof(Index));
        }

        // Eén-klik inschrijven  voor de simpele knop op de lessen-pagina
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SchrijfIn(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsVerwijderd);

            if (les == null)
            {
                TempData["ErrorMessage"] = "Les niet gevonden.";
                return RedirectToAction(nameof(Index));
            }

            // Al ingeschreven?
            var bestaande = await _context.Inschrijvingen
                .FirstOrDefaultAsync(i => i.GebruikerId == userId && i.LesId == id && i.Status == "Actief" && !i.IsVerwijderd);
            if (bestaande != null)
            {
                TempData["ErrorMessage"] = "Je bent al ingeschreven voor deze les.";
                return RedirectToAction(nameof(Index));
            }

            // Les vol?
            if (les.Inschrijvingen.Count(i => i.Status == "Actief") >= les.MaxDeelnemers)
            {
                TempData["ErrorMessage"] = "Deze les is vol.";
                return RedirectToAction(nameof(Index));
            }

            _context.Inschrijvingen.Add(new Inschrijving
            {
                GebruikerId = userId,
                LesId = id,
                InschrijfDatum = DateTime.UtcNow,
                Status = "Actief",
                AangemaaktOp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Je bent ingeschreven voor '{les.Naam}'.";
            return RedirectToAction("Index", "Inschrijvingen");
        }

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

            var existingRegistration = await _context.Inschrijvingen
                .FirstOrDefaultAsync(i => i.GebruikerId == userId && i.LesId == lessonId && i.Status == "Actief");

            if (existingRegistration != null)
            {
                return Json(new { success = false, message = "Je bent al ingeschreven voor deze les" });
            }

            var registeredCount = les.Inschrijvingen.Count(i => i.Status == "Actief");
            if (registeredCount >= les.MaxDeelnemers)
            {
                return Json(new { success = false, message = "Deze les is vol" });
            }

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

        [HttpGet]
        public async Task<IActionResult> LoadSpecialLessons(string type)
        {
            IQueryable<Les> query = _context.Lessen
                .Include(l => l.Inschrijvingen)
                .Where(l => !l.IsVerwijderd && l.IsActief);

            switch (type.ToLower())
            {
                case "today":
                    var today = DateTime.Today;
                    query = query.Where(l => l.StartTijd.Date == today)
                                 .OrderBy(l => l.StartTijd);
                    break;

                case "available":
                    query = query.Where(l => l.StartTijd > DateTime.Now)
                                 .OrderBy(l => l.StartTijd)
                                 .AsEnumerable()
                                 .Where(l =>
                                     l.MaxDeelnemers - l.Inschrijvingen.Count(i => i.Status == "Actief") > 0)
                                 .AsQueryable();
                    break;

                case "myregistrations":
                    var userId = _userManager.GetUserId(User);
                    var registeredLessonIds = await _context.Inschrijvingen
                        .Where(i => i.GebruikerId == userId && i.Status == "Actief")
                        .Select(i => i.LesId)
                        .ToListAsync();

                    query = query.Where(l => registeredLessonIds.Contains(l.Id))
                                 .OrderBy(l => l.StartTijd);
                    break;
            }

            var lessen = await query.ToListAsync();

            ViewData["CurrentSort"] = "";
            ViewData["CurrentFilter"] = "";
            ViewData["CurrentTrainerFilter"] = "all";

            var pageSize = 10;
            var paginatedList = new PaginatedList<Les>(lessen, lessen.Count, 1, pageSize);

            return PartialView("_LessenTable", paginatedList);
        }

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

            var userId = _userManager.GetUserId(User);
            ViewData["IsRegistered"] = await _context.Inschrijvingen
                .AnyAsync(i => i.GebruikerId == userId && i.LesId == id && i.Status == "Actief");

            return View(les);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Standaardwaarden zodat het form geen valid-fouten geeft
            return View(new Les
            {
                IsActief = true,
                MaxDeelnemers = 20,
                StartTijd = DateTime.Today.AddDays(1).AddHours(9),
                EindTijd = DateTime.Today.AddDays(1).AddHours(10),
                Trainer = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Naam,Beschrijving,StartTijd,EindTijd,MaxDeelnemers,Locatie,IsActief")] Les les)
        {
            // Trainer wordt later toegevoegd door de trainer zelf — niet binden vanuit form
            les.Trainer = string.Empty;

            // ModelState kan een fout bevatten voor Trainer  — verwijderen
            ModelState.Remove(nameof(Les.Trainer));

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

        [Authorize(Roles = "Admin")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naam,Beschrijving,StartTijd,EindTijd,MaxDeelnemers,Locatie,Trainer,IsActief,AangemaaktOp")] Les les)
        {
            if (id != les.Id)
            {
                return NotFound();
            }

            // Trainer mag leeg zijn — niet-nullable validatie negeren
            ModelState.Remove(nameof(Les.Trainer));
            les.Trainer ??= string.Empty;

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