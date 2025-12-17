using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikersApiController : ControllerBase
    {
        private readonly IFitnessClubDbContext _context;
        private readonly UserManager<Gebruiker> _userManager;

        public GebruikersApiController(
            IFitnessClubDbContext context, 
            UserManager<Gebruiker> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/GebruikersApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetGebruikers()
        {
            var gebruikers = await _userManager.Users
                .Include(g => g.Abonnement)
                .ToListAsync();

            var result = new List<object>();
            foreach (var gebruiker in gebruikers)
            {
                var rollen = await _userManager.GetRolesAsync(gebruiker);
                result.Add(new
                {
                    gebruiker.Id,
                    gebruiker.UserName,
                    gebruiker.Email,
                    gebruiker.Voornaam,
                    gebruiker.Achternaam,
                    gebruiker.PhoneNumber,
                    Rollen = rollen,
                    Abonnement = gebruiker.Abonnement?.Naam,
                    IsVerwijderd = false
                });
            }

            return Ok(result);
        }

        // GET: api/GebruikersApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetGebruiker(string id)
        {
            var gebruiker = await _userManager.Users
                .Include(g => g.Abonnement)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gebruiker == null)
            {
                return NotFound();
            }

            var rollen = await _userManager.GetRolesAsync(gebruiker);

            return new
            {
                gebruiker.Id,
                gebruiker.UserName,
                gebruiker.Email,
                gebruiker.Voornaam,
                gebruiker.Achternaam,
                gebruiker.PhoneNumber,
                Rollen = rollen,
                Abonnement = gebruiker.Abonnement,
                IsVerwijderd = false
            };
        }
    }
}