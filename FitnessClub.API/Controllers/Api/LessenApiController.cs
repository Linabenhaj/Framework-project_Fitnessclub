using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessClub.Models.Data;
using FitnessClub.Models;

namespace FitnessClub.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessenApiController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;

        public LessenApiController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET api/lessen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLessen()
        {
            var lessen = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .Where(l => l.IsActief && l.StartTijd > DateTime.Now.AddDays(-1))
                .OrderBy(l => l.StartTijd)
                .Select(l => new
                {
                    l.Id,
                    l.Naam,
                    l.Beschrijving,
                    l.StartTijd,
                    l.EindTijd,
                    l.Locatie,
                    l.Trainer,
                    l.MaxDeelnemers,
                    BeschikbarePlaatsen = l.MaxDeelnemers - l.Inschrijvingen.Count(i => i.Status == "Actief"),
                    IsVol = l.MaxDeelnemers - l.Inschrijvingen.Count(i => i.Status == "Actief") <= 0
                })
                .ToListAsync();



            return Ok(lessen);
        }

        // GET api/lessen
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetLes(int id)
        {
            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .ThenInclude(i => i.Gebruiker)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (les == null)
            {
                return NotFound(new { message = "Les niet gevonden" });
            }

            return Ok(new
            {
                les.Id,
                les.Naam,
                les.Beschrijving,
                les.StartTijd,
                les.EindTijd,
                les.Locatie,
                les.Trainer,
                les.MaxDeelnemers,
                DuurMinuten = (int)(les.EindTijd - les.StartTijd).TotalMinutes,
                BeschikbarePlaatsen = les.MaxDeelnemers - les.Inschrijvingen.Count(i => i.Status == "Actief"),
                Inschrijvingen = les.Inschrijvingen.Select(i => new
                {
                    i.Id,
                    GebruikerNaam = i.Gebruiker.Voornaam + " " + i.Gebruiker.Achternaam,
                    i.Status
                })
            });
        }

        // POST api/lessen/{id}/inschrijven
        [HttpPost("{id}/inschrijven")]
        public async Task<ActionResult> Inschrijven(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (les == null)
            {
                return NotFound(new { message = "Les niet gevonden" });
            }


            var aantalIngeschreven = les.Inschrijvingen.Count(i => i.Status == "Actief");
            if (aantalIngeschreven >= les.MaxDeelnemers)
            {
                return BadRequest(new { message = "Les is vol" });
            }

            var bestaandeInschrijving = await _context.Inschrijvingen
                .FirstOrDefaultAsync(i => i.GebruikerId == userId && i.LesId == id && i.Status == "Actief");

            if (bestaandeInschrijving != null)
            {
                return BadRequest(new { message = "Je bent al ingeschreven voor deze les" });
            }

            var inschrijving = new Inschrijving
            {
                GebruikerId = userId,
                LesId = id,
                InschrijfDatum = DateTime.UtcNow,
                Status = "Actief"
            };



            _context.Inschrijvingen.Add(inschrijving);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Succesvol ingeschreven",
                inschrijvingId = inschrijving.Id
            });
        }

        // DELETE api/lessen/{id}/uitschrijven
        [HttpDelete("{id}/uitschrijven")]
        public async Task<ActionResult> Uitschrijven(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var inschrijving = await _context.Inschrijvingen
                .FirstOrDefaultAsync(i => i.GebruikerId == userId && i.LesId == id && i.Status == "Actief");

            if (inschrijving == null)
            {
                return NotFound(new { message = "Inschrijving niet gevonden" });
            }

            var les = await _context.Lessen.FindAsync(id);
            if (les != null && les.StartTijd <= DateTime.Now.AddHours(24))
            {
                return BadRequest(new { message = "Uitschrijven is alleen mogelijk tot 24 uur voor de les" });
            }

            inschrijving.Status = "Geannuleerd";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Succesvol uitgeschreven" });


        }
    }
}