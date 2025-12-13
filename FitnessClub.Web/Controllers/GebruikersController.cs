using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;



namespace FitnessClub.Web.Controllers
{
    public class GebruikersController : Controller
    {
        private readonly FitnessClubDbContext _context;
        private readonly UserManager<Gebruiker> _userManager;

        public GebruikersController(FitnessClubDbContext context, UserManager<Gebruiker> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Gebruikers
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NaamSortParm"] = String.IsNullOrEmpty(sortOrder) ? "naam_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["RolSortParm"] = sortOrder == "Rol" ? "rol_desc" : "Rol";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var gebruikers = from g in _context.Users
                             select g;

            if (!String.IsNullOrEmpty(searchString))
            {
                gebruikers = gebruikers.Where(g => g.Voornaam.Contains(searchString)
                                       || g.Achternaam.Contains(searchString)
                                       || g.Email.Contains(searchString)
                                       || g.Rol.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "naam_desc":
                    gebruikers = gebruikers.OrderByDescending(g => g.Achternaam);
                    break;
                case "Email":
                    gebruikers = gebruikers.OrderBy(g => g.Email);
                    break;
                case "email_desc":
                    gebruikers = gebruikers.OrderByDescending(g => g.Email);
                    break;
                case "Rol":
                    gebruikers = gebruikers.OrderBy(g => g.Rol);
                    break;
                case "rol_desc":
                    gebruikers = gebruikers.OrderByDescending(g => g.Rol);
                    break;
                default:
                    gebruikers = gebruikers.OrderBy(g => g.Achternaam);
                    break;
            }

            int pageSize = 10;

            // GECORRIGEERDE REGEL 89:
            return View(await FitnessClub.Web.PaginatedList<Gebruiker>.CreateAsync(gebruikers.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Gebruikers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gebruiker = await _context.Users
                .Include(g => g.Abonnement)
                .Include(g => g.Inschrijvingen)
                    .ThenInclude(i => i.Les)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gebruiker == null)
            {
                return NotFound();
            }

            return View(gebruiker);
        }

        // GET: Gebruikers/Create
        public IActionResult Create()
        {
            ViewBag.Abonnementen = _context.Abonnementen.ToList();
            return View();
        }

        // POST: Gebruikers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Voornaam,Achternaam,Email,PhoneNumber,Geboortedatum,Rol,AbonnementId")] Gebruiker gebruiker, string password)
        {
            if (ModelState.IsValid)
            {
                gebruiker.UserName = gebruiker.Email;
                gebruiker.EmailConfirmed = true;
                gebruiker.AangemaaktOp = DateTime.UtcNow;

                var result = await _userManager.CreateAsync(gebruiker, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(gebruiker, gebruiker.Rol);
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Abonnementen = _context.Abonnementen.ToList();
            return View(gebruiker);
        }

        // GET: Gebruikers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gebruiker = await _context.Users.FindAsync(id);
            if (gebruiker == null)
            {
                return NotFound();
            }

            ViewBag.Abonnementen = _context.Abonnementen.ToList();
            return View(gebruiker);
        }

        // POST: Gebruikers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Voornaam,Achternaam,Email,PhoneNumber,Geboortedatum,Rol,AbonnementId")] Gebruiker gebruiker)
        {
            if (id != gebruiker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.Voornaam = gebruiker.Voornaam;
                    existingUser.Achternaam = gebruiker.Achternaam;
                    existingUser.Email = gebruiker.Email;
                    existingUser.UserName = gebruiker.Email;
                    existingUser.PhoneNumber = gebruiker.PhoneNumber;
                    existingUser.Geboortedatum = gebruiker.Geboortedatum;
                    existingUser.Rol = gebruiker.Rol;
                    existingUser.AbonnementId = gebruiker.AbonnementId;
                    existingUser.GewijzigdOp = DateTime.UtcNow;

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();

                    // Update role if changed
                    var currentRoles = await _userManager.GetRolesAsync(existingUser);
                    await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                    await _userManager.AddToRoleAsync(existingUser, gebruiker.Rol);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GebruikerExists(gebruiker.Id))
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

            ViewBag.Abonnementen = _context.Abonnementen.ToList();
            return View(gebruiker);
        }

        // GET Gebruikers/Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gebruiker = await _context.Users
                .Include(g => g.Abonnement)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gebruiker == null)
            {
                return NotFound();
            }

            return View(gebruiker);
        }

        // POST Gebruikers/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var gebruiker = await _context.Users.FindAsync(id);
            if (gebruiker != null)
            {
                gebruiker.IsVerwijderd = true;
                gebruiker.VerwijderdOp = DateTime.UtcNow;
                _context.Users.Update(gebruiker);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        private bool GebruikerExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


    }
}
