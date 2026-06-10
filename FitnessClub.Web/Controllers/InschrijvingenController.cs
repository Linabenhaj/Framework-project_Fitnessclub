using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using FitnessClub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Web.Controllers
{
    [Authorize] // Iedereen ingelogd
    public class InschrijvingenController : Controller
    {
        private readonly FitnessClubDbContext _context;
        private readonly UserManager<Gebruiker> _userManager;

        public InschrijvingenController(FitnessClubDbContext context, UserManager<Gebruiker> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Inschrijvingen — admin ziet alles, leden zien alleen eigen
        public async Task<IActionResult> Index(string searchString, string sortOrder, int pageNumber = 1)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["LesSort"]    = string.IsNullOrEmpty(sortOrder) ? "les_desc" : "";
            ViewData["DatumSort"]  = sortOrder == "datum" ? "datum_desc" : "datum";
            ViewData["StatusSort"] = sortOrder == "status" ? "status_desc" : "status";

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            IQueryable<Inschrijving> query = _context.Inschrijvingen
                .Include(i => i.Les)
                .Include(i => i.Gebruiker)
                .Where(i => !i.IsVerwijderd);

            // Leden zien alleen eigen inschrijvingen
            if (!isAdmin)
            {
                query = query.Where(i => i.GebruikerId == userId);
            }

            // Filter op les-naam, status of gebruiker
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(i =>
                    i.Les.Naam.Contains(searchString) ||
                    i.Status.Contains(searchString) ||
                    (i.Gebruiker.Voornaam + " " + i.Gebruiker.Achternaam).Contains(searchString));
            }

            // Sortering
            query = sortOrder switch
            {
                "les_desc"    => query.OrderByDescending(i => i.Les.Naam),
                "datum"       => query.OrderBy(i => i.InschrijfDatum),
                "datum_desc"  => query.OrderByDescending(i => i.InschrijfDatum),
                "status"      => query.OrderBy(i => i.Status),
                "status_desc" => query.OrderByDescending(i => i.Status),
                _             => query.OrderBy(i => i.Les.Naam)
            };

            const int pageSize = 10;
            var model = await PaginatedList<Inschrijving>.CreateAsync(query, pageNumber, pageSize);

            // Alleen admin: lijst van lessen waarvoor een trainer zich heeft aangegeven
            if (isAdmin)
            {
                ViewBag.LessenMetTrainer = await _context.Lessen
                    .Where(l => !l.IsVerwijderd && !string.IsNullOrEmpty(l.Trainer))
                    .OrderBy(l => l.StartTijd)
                    .ToListAsync();
            }

            return View(model);
        }

        // AJAX-endpoint: levert alleen de tabel als partial view (Unobtrusive Ajax)
        [HttpGet]
        public async Task<IActionResult> LoadInschrijvingenPartial(string searchString, string sortOrder, int pageNumber = 1)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["LesSort"]    = string.IsNullOrEmpty(sortOrder) ? "les_desc" : "";
            ViewData["DatumSort"]  = sortOrder == "datum" ? "datum_desc" : "datum";
            ViewData["StatusSort"] = sortOrder == "status" ? "status_desc" : "status";

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            IQueryable<Inschrijving> query = _context.Inschrijvingen
                .Include(i => i.Les)
                .Include(i => i.Gebruiker)
                .Where(i => !i.IsVerwijderd);

            if (!isAdmin)
                query = query.Where(i => i.GebruikerId == userId);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(i =>
                    i.Les.Naam.Contains(searchString) ||
                    i.Status.Contains(searchString) ||
                    (i.Gebruiker.Voornaam + " " + i.Gebruiker.Achternaam).Contains(searchString));
            }

            query = sortOrder switch
            {
                "les_desc"    => query.OrderByDescending(i => i.Les.Naam),
                "datum"       => query.OrderBy(i => i.InschrijfDatum),
                "datum_desc"  => query.OrderByDescending(i => i.InschrijfDatum),
                "status"      => query.OrderBy(i => i.Status),
                "status_desc" => query.OrderByDescending(i => i.Status),
                _             => query.OrderBy(i => i.Les.Naam)
            };

            const int pageSize = 10;
            var model = await PaginatedList<Inschrijving>.CreateAsync(query, pageNumber, pageSize);
            return PartialView("_inschrijvingentable", model);
        }

        // GET: Inschrijvingen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var inschrijving = await _context.Inschrijvingen
                .Include(i => i.Les)
                .Include(i => i.Gebruiker)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inschrijving == null) return NotFound();

            // Leden mogen alleen eigen inschrijvingen zien
            if (!User.IsInRole("Admin") && inschrijving.GebruikerId != _userManager.GetUserId(User))
                return Forbid();

            return View(inschrijving);
        }

        // GET: Inschrijvingen/Create — leden kunnen zelf inschrijven, admin ook
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new Inschrijving { InschrijfDatum = DateTime.UtcNow, Status = "Actief" });
        }

        // POST: Inschrijvingen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LesId,GebruikerId,InschrijfDatum,Status,Opmerkingen")] Inschrijving inschrijving)
        {
            // Niet-admins kunnen alleen voor zichzelf inschrijven
            if (!User.IsInRole("Admin"))
            {
                inschrijving.GebruikerId = _userManager.GetUserId(User)!;
            }

            // Validatie: dubbele inschrijving?
            var bestaat = await _context.Inschrijvingen.AnyAsync(i =>
                i.LesId == inschrijving.LesId &&
                i.GebruikerId == inschrijving.GebruikerId &&
                i.Status == "Actief" &&
                !i.IsVerwijderd);

            if (bestaat)
            {
                ModelState.AddModelError("", "Deze gebruiker is al ingeschreven voor deze les.");
            }

            // Validatie: is de les niet vol?
            var les = await _context.Lessen.Include(l => l.Inschrijvingen).FirstOrDefaultAsync(l => l.Id == inschrijving.LesId);
            if (les != null && les.Inschrijvingen.Count(i => i.Status == "Actief") >= les.MaxDeelnemers)
            {
                ModelState.AddModelError("", "Deze les is helaas vol.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(inschrijving);
            }

            try
            {
                inschrijving.AangemaaktOp = DateTime.UtcNow;
                _context.Inschrijvingen.Add(inschrijving);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Inschrijving succesvol aangemaakt.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout: {ex.Message}";
                await PopulateDropdowns();
                return View(inschrijving);
            }
        }

        // GET: Inschrijvingen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var inschrijving = await _context.Inschrijvingen
                .Include(i => i.Les)
                .Include(i => i.Gebruiker)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inschrijving == null) return NotFound();

            // Leden mogen alleen eigen inschrijvingen verwijderen
            if (!User.IsInRole("Admin") && inschrijving.GebruikerId != _userManager.GetUserId(User))
                return Forbid();

            return View(inschrijving);
        }

        // POST: Inschrijvingen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return await UitschrijfInternalAsync(id);
        }

        // POST: Inschrijvingen/Uitschrijf/5 — één-klik uitschrijven (hard delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uitschrijf(int id)
        {
            return await UitschrijfInternalAsync(id);
        }

        private async Task<IActionResult> UitschrijfInternalAsync(int id)
        {
            var inschrijving = await _context.Inschrijvingen.FindAsync(id);
            if (inschrijving == null) return NotFound();

            if (!User.IsInRole("Admin") && inschrijving.GebruikerId != _userManager.GetUserId(User))
                return Forbid();

            try
            {
                _context.Inschrijvingen.Remove(inschrijving);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Je bent uitgeschreven voor deze les.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper: vul dropdowns voor Lessen + Gebruikers
        private async Task PopulateDropdowns()
        {
            var lessen = await _context.Lessen
                .Where(l => l.IsActief && !l.IsVerwijderd && l.StartTijd > DateTime.Now)
                .OrderBy(l => l.StartTijd)
                .Select(l => new { l.Id, Display = $"{l.Naam} - {l.StartTijd:dd/MM/yyyy HH:mm}" })
                .ToListAsync();
            ViewBag.LessenList = new SelectList(lessen, "Id", "Display");

            // Alleen admin krijgt gebruikerskeuze
            if (User.IsInRole("Admin"))
            {
                var gebruikers = await _userManager.Users
                    .OrderBy(u => u.Voornaam)
                    .Select(u => new { u.Id, Display = $"{u.Voornaam} {u.Achternaam} ({u.Email})" })
                    .ToListAsync();
                ViewBag.GebruikersList = new SelectList(gebruikers, "Id", "Display");
            }
        }
    }
}
