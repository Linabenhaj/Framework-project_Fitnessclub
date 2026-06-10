using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GebruikersController : Controller
    {
        private readonly UserManager<Gebruiker> _userManager;
        private readonly IFitnessClubDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GebruikersController(
            UserManager<Gebruiker> userManager,
            IFitnessClubDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        // GET: Gebruikers  overzicht met filter
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var query = _userManager.Users.Include(u => u.Abonnement).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(u =>
                    (u.Voornaam + " " + u.Achternaam).Contains(searchString) ||
                    u.Email!.Contains(searchString));
            }

            var gebruikers = await query.OrderBy(u => u.Voornaam).ToListAsync();

            var viewData = new List<GebruikerViewItem>();
            foreach (var g in gebruikers)
            {
                var rollen = await _userManager.GetRolesAsync(g);
                viewData.Add(new GebruikerViewItem
                {
                    Gebruiker = g,
                    Rol = rollen.FirstOrDefault() ?? "—"
                });
            }

            return View(viewData);
        }

        // GET: Gebruikers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var gebruiker = await _userManager.Users
                .Include(g => g.Abonnement)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gebruiker == null) return NotFound();

            ViewBag.Rollen = await _userManager.GetRolesAsync(gebruiker);
            return View(gebruiker);
        }

        // GET: Gebruikers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var gebruiker = await _userManager.Users.FirstOrDefaultAsync(g => g.Id == id);
            if (gebruiker == null) return NotFound();

            var alleRollen = _roleManager.Roles.Select(r => r.Name!).ToList();
            var huidigeRol = (await _userManager.GetRolesAsync(gebruiker)).FirstOrDefault();
            ViewBag.RollenList = new SelectList(alleRollen, huidigeRol);

            var abonnementen = await _context.Abonnementen.ToListAsync();
            ViewBag.AbonnementenList = new SelectList(abonnementen, "Id", "Naam", gebruiker.AbonnementId);

            return View(gebruiker);
        }

        // POST: Gebruikers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string Voornaam, string Achternaam, string Email, string? Telefoon, int? AbonnementId, string Rol)
        {
            var gebruiker = await _userManager.FindByIdAsync(id);
            if (gebruiker == null) return NotFound();

            try
            {
                gebruiker.Voornaam = Voornaam;
                gebruiker.Achternaam = Achternaam;
                gebruiker.Email = Email;
                gebruiker.UserName = Email; // UserName is gekoppeld aan email in Identity
                gebruiker.PhoneNumber = Telefoon;
                gebruiker.AbonnementId = AbonnementId;
                gebruiker.GewijzigdOp = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(gebruiker);
                if (!updateResult.Succeeded)
                {
                    TempData["ErrorMessage"] = "Kon gebruiker niet bijwerken: " +
                        string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Edit), new { id });
                }

                if (!string.IsNullOrEmpty(Rol))
                {
                    var huidigeRollen = await _userManager.GetRolesAsync(gebruiker);
                    if (huidigeRollen.Any())
                        await _userManager.RemoveFromRolesAsync(gebruiker, huidigeRollen);
                    await _userManager.AddToRoleAsync(gebruiker, Rol);
                }

                TempData["SuccessMessage"] = $"Gebruiker '{gebruiker.Voornaam} {gebruiker.Achternaam}' bijgewerkt.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout: {ex.Message}";
                return RedirectToAction(nameof(Edit), new { id });
            }
        }

        // POST: Gebruikers/Delete/5 — HARD delete (echt uit DB)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var gebruiker = await _userManager.FindByIdAsync(id);
            if (gebruiker == null) return NotFound();

            if (gebruiker.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "Je kunt jezelf niet verwijderen.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _userManager.DeleteAsync(gebruiker);
                if (result.Succeeded)
                    TempData["SuccessMessage"] = "Gebruiker definitief verwijderd.";
                else
                    TempData["ErrorMessage"] = "Kon gebruiker niet verwijderen: " +
                        string.Join(", ", result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class GebruikerViewItem
    {
        public Gebruiker Gebruiker { get; set; } = null!;
        public string Rol { get; set; } = "";
    }
}
