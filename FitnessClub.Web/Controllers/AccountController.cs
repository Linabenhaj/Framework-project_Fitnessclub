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
    public class AccountController : Controller //centraliseert alle authenticatie
    {
        private readonly UserManager<Gebruiker> _userManager;
        private readonly SignInManager<Gebruiker> _signInManager;
        private readonly FitnessClubDbContext _context;

        public AccountController(
            UserManager<Gebruiker> userManager,
            SignInManager<Gebruiker> signInManager,
            FitnessClubDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            await PopulateAbonnementenAsync();
            return View();
        }

        private async Task PopulateAbonnementenAsync(int? selected = null)
        {
            var abonnementen = await _context.Abonnementen
                .Where(a => a.IsActief && !a.IsVerwijderd)
                .OrderBy(a => a.Prijs)
                .Select(a => new
                {
                    a.Id,
                    Display = $"{a.Naam} — € {a.Prijs:F2} / maand"
                })
                .ToListAsync();
            ViewBag.AbonnementenList = new SelectList(abonnementen, "Id", "Display", selected);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model) // maken van nieuwe gebruiker en meteen inloggen, met validatie van voorwaarden
        {
            // Voorwaarden-validatie handmatig 
            if (!model.AccepteerVoorwaarden)
            {
                ModelState.AddModelError(nameof(model.AccepteerVoorwaarden),
                    "Je moet akkoord gaan met de voorwaarden om je te registreren.");
            }

            if (ModelState.IsValid)
            {
                var user = new Gebruiker
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Voornaam = model.Voornaam,
                    Achternaam = model.Achternaam,
                    PhoneNumber = model.Telefoonnummer,
                    AbonnementId = model.AbonnementId  // bij registratie meteen een abo kiezen
                };

                var result = await _userManager.CreateAsync(user, model.Wachtwoord);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Lid"); // rol toegeknd bijregistratie
                    await _signInManager.SignInAsync(user, isPersistent: false); // meteen inloggen na registratie
                    TempData["SuccessMessage"] = "Welkom! Je account is aangemaakt.";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await PopulateAbonnementenAsync(model.AbonnementId);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(  //!
                    model.Email, model.Wachtwoord, model.Onthouden, false); // false = geen lockout bij mislukte pogingen

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Succesvol ingelogd!";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Ongeldige login poging. Controleer je e-mail en wachtwoord.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() //Logout maakt SignOutAsync aan en verwijdert het cookie
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Je bent uitgelogd.";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Profile() // toont de profielpagina, WijzigAbonnement laat de gebruiker zijn abonnement veranderen
        {
            // Admins hebben geen profielpagina  zij gebruiken Gebruikers-beheer
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Home");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            // Herlaad met Abonnement
            user = await _context.Gebruikers
                .Include(g => g.Abonnement)
                .FirstOrDefaultAsync(g => g.Id == user.Id);

            // Lijst met beschikbare abonnementen om uit te kiezen
            var abonnementen = await _context.Abonnementen
                .Where(a => a.IsActief && !a.IsVerwijderd)
                .OrderBy(a => a.Prijs)
                .Select(a => new
                {
                    a.Id,
                    Display = $"{a.Naam} — € {a.Prijs:F2} / maand"
                })
                .ToListAsync();
            ViewBag.AbonnementenList = new SelectList(abonnementen, "Id", "Display", user!.AbonnementId);

            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WijzigAbonnement(int? abonnementId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            try
            {
                user.AbonnementId = abonnementId;
                user.GewijzigdOp = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (abonnementId.HasValue)
                    {
                        var abo = await _context.Abonnementen.FindAsync(abonnementId.Value);
                        TempData["SuccessMessage"] = $"Je abonnement is gewijzigd naar '{abo?.Naam}'.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Je abonnement is verwijderd.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Kon abonnement niet wijzigen.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fout: {ex.Message}";
            }

            return RedirectToAction(nameof(Profile));
        }
    }
}