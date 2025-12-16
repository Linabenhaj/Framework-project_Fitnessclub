using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessClub.API.Controllers
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

        // GET: api/lessen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Les>>> GetLessen()
        {
            var lessen = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .Where(l => l.IsActief && l.StartTijd > DateTime.Now.AddDays(-1)) // Laat ook recente lessen zien
                .OrderBy(l => l.StartTijd)
                .ToListAsync();

            return Ok(lessen);
        }

        // POST: api/lessen/{id}/inschrijven
        [HttpPost("{id}/inschrijven")]
        public async Task<IActionResult> InschrijvenVoorLes(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (les == null)
                return NotFound(new { Message = "Les niet gevonden" });

            // Check of gebruiker al ingeschreven is
            var bestaandeInschrijving = await _context.Inschrijvingen
                .FirstOrDefaultAsync(i => i.LesId == id && i.GebruikerId == userId && i.Status == "Actief");

            if (bestaandeInschrijving != null)
                return BadRequest(new { Message = "Je bent al ingeschreven voor deze les" });

            // Check of er nog plaats is
            if (les.Inschrijvingen.Count(i => i.Status == "Actief") >= les.MaxDeelnemers)
                return BadRequest(new { Message = "Les is vol" });

            // Check of les in de toekomst is
            if (les.StartTijd <= DateTime.Now)
                return BadRequest(new { Message = "Les is al begonnen" });

            // Voeg inschrijving toe
            var inschrijving = new Inschrijving
            {
                LesId = id,
                GebruikerId = userId,
                InschrijfDatum = DateTime.Now,
                Status = "Actief"
            };

            _context.Inschrijvingen.Add(inschrijving);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Succesvol ingeschreven",
                InschrijvingId = inschrijving.Id
            });
        }

        // POST: api/lessen/{id}/uitschrijven
        [HttpPost("{id}/uitschrijven")]
        public async Task<IActionResult> UitschrijvenVoorLes(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var inschrijving = await _context.Inschrijvingen
                .Include(i => i.Les)
                .FirstOrDefaultAsync(i => i.LesId == id && i.GebruikerId == userId && i.Status == "Actief");

            if (inschrijving == null)
                return NotFound(new { Message = "Inschrijving niet gevonden" });

            // Check of uitschrijven nog mag (minstens 24 uur van tevoren)
            if (inschrijving.Les.StartTijd <= DateTime.Now.AddHours(24))
                return BadRequest(new { Message = "Uitschrijven is alleen mogelijk tot 24 uur voor de les" });

            // Update status
            inschrijving.Status = "Geannuleerd";
            _context.Inschrijvingen.Update(inschrijving);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Succesvol uitgeschreven"
            });
        }
    }
}