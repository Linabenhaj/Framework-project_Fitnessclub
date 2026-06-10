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
    [Authorize(AuthenticationSchemes = "Bearer")] 
    public class LessenApiController : ControllerBase
    {
        private readonly IFitnessClubDbContext _context;
        private readonly ILogger<LessenApiController> _logger;

        public LessenApiController(
            IFitnessClubDbContext context,
            ILogger<LessenApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/lessen
        [HttpGet]
        [AllowAnonymous] // Toegestaan zonder authenticatie voor bekijken
        public async Task<ActionResult<IEnumerable<object>>> GetLessen(
            [FromQuery] string search = "",
            [FromQuery] string trainer = "",
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] bool? isActive = true)
        {
            try
            {
                var query = _context.Lessen
                    .Include(l => l.Inschrijvingen)
                    .Where(l => l.IsActief);

                // Filters
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(l => l.Naam.Contains(search) ||
                                            l.Beschrijving.Contains(search) ||
                                            l.Locatie.Contains(search));
                }

                if (!string.IsNullOrEmpty(trainer))
                {
                    query = query.Where(l => l.Trainer == trainer);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(l => l.StartTijd >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(l => l.StartTijd <= toDate.Value);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(l => l.IsActief == isActive.Value);
                }

                var lessen = await query
                    .OrderBy(l => l.StartTijd)
                    .Select(l => new
                    {
                        l.Id,
                        l.Naam,
                        l.Beschrijving,
                        StartTijd = l.StartTijd.ToString("yyyy-MM-ddTHH:mm:ss"),
                        EindTijd = l.EindTijd.ToString("yyyy-MM-ddTHH:mm:ss"),
                        l.Locatie,
                        l.Trainer,
                        l.MaxDeelnemers,
                        AvailableSpots = l.MaxDeelnemers - l.Inschrijvingen.Count(i => i.Status == "Actief"),
                        IsFull = l.MaxDeelnemers - l.Inschrijvingen.Count(i => i.Status == "Actief") <= 0,
                        l.IsActief,
                        LastSynced = DateTime.UtcNow
                    })
                    .ToListAsync();

                _logger.LogInformation($"API: {lessen.Count} lessen opgehaald");

                return Ok(new
                {
                    success = true,
                    count = lessen.Count,
                    data = lessen,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen lessen via API");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Interne server fout",
                    error = ex.Message
                });
            }
        }

        // GET: api/lessen/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetLes(int id)
        {
            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                .FirstOrDefaultAsync(l => l.Id == id && l.IsActief);

            if (les == null)
            {
                return NotFound(new { success = false, message = "Les niet gevonden" });
            }

            var availableSpots = les.MaxDeelnemers -
                               les.Inschrijvingen.Count(i => i.Status == "Actief");

            return Ok(new
            {
                success = true,
                data = new
                {
                    les.Id,
                    les.Naam,
                    les.Beschrijving,
                    StartTijd = les.StartTijd.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EindTijd = les.EindTijd.ToString("yyyy-MM-ddTHH:mm:ss"),
                    les.Locatie,
                    les.Trainer,
                    les.MaxDeelnemers,
                    AvailableSpots = availableSpots,
                    IsFull = availableSpots <= 0,
                    les.IsActief,
                    Duration = (les.EindTijd - les.StartTijd).TotalMinutes,
                    LastSynced = DateTime.UtcNow
                }
            });
        }

        // POST: api/lessen/5/inschrijven
        [HttpPost("{id}/inschrijven")]
        public async Task<IActionResult> InschrijvenVoorLes(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("API: Inschrijven mislukt - geen gebruiker ID");
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Niet geautoriseerd"
                    });
                }

                var les = await _context.Lessen
                    .Include(l => l.Inschrijvingen)
                    .FirstOrDefaultAsync(l => l.Id == id && l.IsActief);

                if (les == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Les niet gevonden"
                    });
                }

                // Check of al ingeschreven
                var bestaandeInschrijving = await _context.Inschrijvingen
                    .FirstOrDefaultAsync(i => i.LesId == id &&
                                             i.GebruikerId == userId &&
                                             i.Status == "Actief");

                if (bestaandeInschrijving != null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Je bent al ingeschreven voor deze les"
                    });
                }

                // Check beschikbare plaatsen
                var registeredCount = les.Inschrijvingen.Count(i => i.Status == "Actief");
                if (registeredCount >= les.MaxDeelnemers)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Les is vol"
                    });
                }

                // Check of les nog niet begonnen is
                if (les.StartTijd <= DateTime.Now)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Les is al begonnen"
                    });
                }

                // Maak inschrijving
                var inschrijving = new Inschrijving
                {
                    LesId = id,
                    GebruikerId = userId,
                    InschrijfDatum = DateTime.Now,
                    Status = "Actief",
                    AangemaaktOp = DateTime.Now
                };

                await _context.Inschrijvingen.AddAsync(inschrijving);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"API: Gebruiker {userId} ingeschreven voor les {id}");

                // Update beschikbare plaatsen
                var newAvailableSpots = les.MaxDeelnemers - (registeredCount + 1);

                return Ok(new
                {
                    success = true,
                    message = "Succesvol ingeschreven",
                    inschrijvingId = inschrijving.Id,
                    availableSpots = newAvailableSpots,
                    isFull = newAvailableSpots <= 0,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API: Fout bij inschrijven voor les {id}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Interne server fout"
                });
            }
        }

        // POST: api/lessen/5/uitschrijven
        [HttpPost("{id}/uitschrijven")]
        public async Task<IActionResult> UitschrijvenVoorLes(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Niet geautoriseerd"
                    });
                }

                var inschrijving = await _context.Inschrijvingen
                    .Include(i => i.Les)
                    .FirstOrDefaultAsync(i => i.LesId == id &&
                                             i.GebruikerId == userId &&
                                             i.Status == "Actief");

                if (inschrijving == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Inschrijving niet gevonden"
                    });
                }

                // Check annuleringsvoorwaarden (24 uur van tevoren)
                if (inschrijving.Les.StartTijd <= DateTime.Now.AddHours(24))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Uitschrijven is alleen mogelijk tot 24 uur voor de les"
                    });
                }

                // Update status
                inschrijving.Status = "Geannuleerd";
                inschrijving.GewijzigdOp = DateTime.Now;

                _context.Inschrijvingen.Update(inschrijving);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"API: Gebruiker {userId} uitgeschreven voor les {id}");

                return Ok(new
                {
                    success = true,
                    message = "Succesvol uitgeschreven",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API: Fout bij uitschrijven voor les {id}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Interne server fout"
                });
            }
        }

        // GET: api/lessen/mijninschrijvingen
        [HttpGet("mijninschrijvingen")]
        public async Task<ActionResult<IEnumerable<object>>> GetMijnInschrijvingen()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Niet geautoriseerd"
                });
            }

            var inschrijvingen = await _context.Inschrijvingen
                .Include(i => i.Les)
                .Where(i => i.GebruikerId == userId &&
                           i.Status == "Actief" &&
                           i.Les.IsActief)
                .OrderBy(i => i.Les.StartTijd)
                .Select(i => new
                {
                    i.Id,
                    Les = new
                    {
                        i.Les.Id,
                        i.Les.Naam,
                        StartTijd = i.Les.StartTijd.ToString("yyyy-MM-ddTHH:mm:ss"),
                        i.Les.Locatie,
                        i.Les.Trainer
                    },
                    i.InschrijfDatum,
                    i.Status
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = inschrijvingen.Count,
                data = inschrijvingen,
                timestamp = DateTime.UtcNow
            });
        }
    }
}