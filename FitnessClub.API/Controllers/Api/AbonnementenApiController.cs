using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;

namespace FitnessClub.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AbonnementenApiController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;

        public AbonnementenApiController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET api/abonnementen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAbonnementen()
        {
            var abonnementen = await _context.Abonnementen
                .Where(a => a.IsActief)
                .Select(a => new
                {
                    a.Id,
                    a.Naam,
                    a.Prijs,
                    a.Omschrijving,
                    a.LooptijdMaanden,
                    AantalGebruikers = _context.Users.Count(u => u.AbonnementId == a.Id)  // ← FIX
                })
                .ToListAsync();

            return Ok(abonnementen);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAbonnement(int id)
        {
            var abonnement = await _context.Abonnementen
                .FirstOrDefaultAsync(a => a.Id == id);  // ← FIX: verwijderde .Include

            if (abonnement == null)
            {
                return NotFound(new { message = "Abonnement niet gevonden" });
            }

            // Haal gebruikers apart op
            var gebruikers = await _context.Users
                .Where(u => u.AbonnementId == id)
                .Select(g => new
                {
                    g.Id,
                    Naam = g.Voornaam + " " + g.Achternaam,
                    g.Email
                })
                .ToListAsync();

            return Ok(new
            {
                abonnement.Id,
                abonnement.Naam,
                abonnement.Prijs,
                abonnement.Omschrijving,
                abonnement.LooptijdMaanden,
                Gebruikers = gebruikers  // ← FIX: apart opgehaald
            });
        }
    }
}