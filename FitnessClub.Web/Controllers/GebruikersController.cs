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
        private readonly FitnessClubDbContext _context;  // Wijzig naar ApplicationDbContext
        private readonly RoleManager<IdentityRole> _roleManager;

        public GebruikersController(
            UserManager<Gebruiker> userManager,
            FitnessClubDbContext context,  // Wijzig naar ApplicationDbContext
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

            // Maak een lijst met gebruikers en hun rollen
            var gebruikersMetRollen = new List<object>();

            foreach (var gebruiker in gebruikers)
            {
                var rollen = await _userManager.GetRolesAsync(gebruiker);
                gebruikersMetRollen.Add(new
                {
                    Gebruiker = gebruiker,
                    Rollen = rollen.FirstOrDefault() // Of toon alle rollen
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

            // Haal rollen op voor deze gebruiker
            var rollen = await _userManager.GetRolesAsync(gebruiker);
            ViewBag.Rollen = rollen;

            return View(gebruiker);
        }


        // Andere methods...

        // Helper method voor rollen
        private async Task<List<string>> GetUserRolesAsync(Gebruiker gebruiker)
        {
            return (await _userManager.GetRolesAsync(gebruiker)).ToList();
        }
    }
}