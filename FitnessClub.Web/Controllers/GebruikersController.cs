using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub.Web.Controllers
{
    public class GebruikersController : Controller
    {
        private readonly UserManager<Gebruiker> _userManager;
        private readonly IFitnessClubDbContext _context;  // Interface gebruiken!
        private readonly RoleManager<IdentityRole> _roleManager;

        public GebruikersController(
            UserManager<Gebruiker> userManager,
            IFitnessClubDbContext context,  // Interface gebruiken!
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        // GET: Gebruikers
        public async Task<IActionResult> Index()
        {
            var gebruikers = await _userManager.Users
                .Include(g => g.Abonnement)
                .ToListAsync();

            var gebruikersMetRollen = new List<object>();

            foreach (var gebruiker in gebruikers)
            {
                var rollen = await _userManager.GetRolesAsync(gebruiker);
                gebruikersMetRollen.Add(new
                {
                    Gebruiker = gebruiker,
                    Rollen = rollen.FirstOrDefault()
                });
            }

            return View(gebruikersMetRollen);
        }

        // GET: Gebruikers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gebruiker = await _userManager.Users
                .Include(g => g.Abonnement)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gebruiker == null)
            {
                return NotFound();
            }

            var rollen = await _userManager.GetRolesAsync(gebruiker);
            ViewBag.Rollen = rollen;

            return View(gebruiker);
        }

        // Helper method
        private async Task<List<string>> GetUserRolesAsync(Gebruiker gebruiker)
        {
            return (await _userManager.GetRolesAsync(gebruiker)).ToList();
        }
    }
}