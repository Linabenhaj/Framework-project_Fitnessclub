using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using FitnessClub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Web.Controllers
{
    [Authorize] // Alleen ingelogde gebruikers
    public class AbonnementenController : Controller
    {
        private readonly FitnessClubDbContext _context;

        public AbonnementenController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET: Abonnementen (zichtbaar voor iedereen die ingelogd is)
        // Filteren op naam
        public async Task<IActionResult> Index(string searchString, string sortOrder, int pageNumber = 1)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["NaamSort"]   = string.IsNullOrEmpty(sortOrder) ? "naam_desc" : "";
            ViewData["PrijsSort"]  = sortOrder == "prijs" ? "prijs_desc" : "prijs";
            ViewData["DuurSort"]   = sortOrder == "duur" ? "duur_desc" : "duur";

            var query = _context.Abonnementen.Where(a => !a.IsVerwijderd).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(a =>
                    a.Naam.Contains(searchString) ||
                    a.Type.Contains(searchString));
            }

            query = sortOrder switch
            {
                "naam_desc"  => query.OrderByDescending(a => a.Naam),
                "prijs"      => query.OrderBy(a => a.Prijs),
                "prijs_desc" => query.OrderByDescending(a => a.Prijs),
                "duur"       => query.OrderBy(a => a.DuurInMaanden),
                "duur_desc"  => query.OrderByDescending(a => a.DuurInMaanden),
                _            => query.OrderBy(a => a.Naam)
            };

            const int pageSize = 10;
            var model = await PaginatedList<Abonnement>.CreateAsync(query, pageNumber, pageSize);
            return View(model);
        }

        // GET: Abonnementen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var abonnement = await _context.Abonnementen
                .FirstOrDefaultAsync(a => a.Id == id);

            if (abonnement == null) return NotFound();
            return View(abonnement);
        }
        //alleen admins mogen abonnementen aanmaken, bewerken en verwijderen
        // GET: Abonnementen/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new Abonnement { IsActief = true, DuurInMaanden = 1 });
        }

        // POST: Abonnementen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Naam,Beschrijving,Type,Prijs,DuurInMaanden,IsActief")] Abonnement abonnement)
        {
            if (!ModelState.IsValid)
                return View(abonnement);

            try
            {
                _context.Abonnementen.Add(abonnement);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Abonnement '{abonnement.Naam}' succesvol aangemaakt.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout bij aanmaken: {ex.Message}";
                return View(abonnement);
            }
        }

        // GET: Abonnementen/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var abonnement = await _context.Abonnementen.FindAsync(id);
            if (abonnement == null) return NotFound();
            return View(abonnement);
        }

        // POST: Abonnementen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naam,Beschrijving,Type,Prijs,DuurInMaanden,IsActief")] Abonnement abonnement)
        {
            if (id != abonnement.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(abonnement);

            try
            {
                _context.Update(abonnement);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Abonnement '{abonnement.Naam}' succesvol bijgewerkt.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AbonnementBestaat(abonnement.Id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout bij bewerken: {ex.Message}";
                return View(abonnement);
            }
        }

        // GET: Abonnementen/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var abonnement = await _context.Abonnementen
                .FirstOrDefaultAsync(a => a.Id == id);

            if (abonnement == null) return NotFound();
            return View(abonnement);
        }

        // POST: Abonnementen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var abonnement = await _context.Abonnementen.FindAsync(id);
                if (abonnement != null)
                {
                    // Soft delete: zet IsActief = false in plaats van fysiek verwijderen
                    abonnement.IsActief = false;
                    abonnement.IsVerwijderd = true;
                    abonnement.GewijzigdOp = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Abonnement '{abonnement.Naam}' is verwijderd.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout bij verwijderen: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<bool> AbonnementBestaat(int id)
        {
            return await _context.Abonnementen.AnyAsync(a => a.Id == id);
        }
    }
}
