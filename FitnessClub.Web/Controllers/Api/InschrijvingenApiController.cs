using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using System.Text.Json;

namespace FitnessClub.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InschrijvingenApiController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        public InschrijvingenApiController(FitnessClubDbContext context)
        {
            _context = context;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        // GET api/inschrijvingen
        [HttpGet]
        [ResponseCache(Duration = 30)] // Caching header
        public async Task<ActionResult> GetInschrijvingen(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sort = "InschrijfDatum",
            [FromQuery] bool descending = true)
        {
            try
            {
                var query = _context.Inschrijvingen
                    .Include(i => i.Gebruiker)
                    .Include(i => i.Les)
                    .Where(i => i.Status == "Actief")
                    .AsQueryable();

                // Apply sorting
                query = descending ?
                    query.OrderByDescending(GetSortExpression(sort)) :
                    query.OrderBy(GetSortExpression(sort));

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply pagination
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new
                    {
                        i.Id,
                        i.InschrijfDatum,
                        i.Status,
                        Gebruiker = new
                        {
                            i.Gebruiker.Id,
                            Naam = i.Gebruiker.Voornaam + " " + i.Gebruiker.Achternaam,
                            i.Gebruiker.Email
                        },
                        Les = new
                        {
                            i.Les.Id,
                            i.Les.Naam,
                            i.Les.StartTijd,
                            i.Les.EindTijd,
                            i.Les.Locatie,
                            i.Les.Trainer
                        }
                    })
                    .ToListAsync();

                // Add pagination headers
                Response.Headers.Append("X-Pagination-TotalCount", totalCount.ToString());
                Response.Headers.Append("X-Pagination-PageCount",
                    Math.Ceiling(totalCount / (double)pageSize).ToString());
                Response.Headers.Append("X-Pagination-Page", page.ToString());
                Response.Headers.Append("X-Pagination-PageSize", pageSize.ToString());

                return Ok(new
                {
                    Items = items,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // GET api/inschrijvingen/user/{userId}
        [HttpGet("user/{userId}")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult> GetUserInschrijvingen(
            string userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Inschrijvingen
                    .Include(i => i.Les)
                    .Where(i => i.GebruikerId == userId && i.Status == "Actief")
                    .OrderByDescending(i => i.Les.StartTijd);

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new
                    {
                        i.Id,
                        i.InschrijfDatum,
                        i.Status,
                        Les = new
                        {
                            i.Les.Id,
                            i.Les.Naam,
                            i.Les.StartTijd,
                            i.Les.EindTijd,
                            i.Les.Locatie,
                            i.Les.Trainer,
                            IsVol = i.Les.Inschrijvingen.Count >= i.Les.MaxDeelnemers
                        }
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Items = items,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // GET api/inschrijvingen/les/{lesId}
        [HttpGet("les/{lesId}")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult> GetLesInschrijvingen(int lesId)
        {
            try
            {
                var inschrijvingen = await _context.Inschrijvingen
                    .Include(i => i.Gebruiker)
                    .Where(i => i.LesId == lesId && i.Status == "Actief")
                    .OrderBy(i => i.InschrijfDatum)
                    .Select(i => new
                    {
                        i.Id,
                        i.InschrijfDatum,
                        Gebruiker = new
                        {
                            i.Gebruiker.Id,
                            Naam = i.Gebruiker.Voornaam + " " + i.Gebruiker.Achternaam,
                            i.Gebruiker.Email,
                            i.Gebruiker.PhoneNumber
                        }
                    })
                    .ToListAsync();

                return Ok(inschrijvingen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // POST api/inschrijvingen
        [HttpPost]
        public async Task<ActionResult> CreateInschrijving([FromBody] CreateInschrijvingRequest request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Niet geautoriseerd" });
                }

                // Check if already registered
                var existing = await _context.Inschrijvingen
                    .FirstOrDefaultAsync(i => i.GebruikerId == userId &&
                                              i.LesId == request.LesId &&
                                              i.Status == "Actief");

                if (existing != null)
                {
                    return BadRequest(new { message = "Al ingeschreven voor deze les" });
                }

                // Check if lesson exists and has space
                var les = await _context.Lessen
                    .Include(l => l.Inschrijvingen)
                    .FirstOrDefaultAsync(l => l.Id == request.LesId);

                if (les == null)
                {
                    return NotFound(new { message = "Les niet gevonden" });
                }

                if (les.Inschrijvingen.Count(i => i.Status == "Actief") >= les.MaxDeelnemers)
                {
                    return BadRequest(new { message = "Les is vol" });
                }

                var inschrijving = new Inschrijving
                {
                    GebruikerId = userId,
                    LesId = request.LesId,
                    InschrijfDatum = DateTime.UtcNow,
                    Status = "Actief",
                    Opmerkingen = request.Opmerkingen
                };

                _context.Inschrijvingen.Add(inschrijving);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserInschrijvingen),
                    new { userId = userId },
                    new
                    {
                        message = "Succesvol ingeschreven",
                        inschrijvingId = inschrijving.Id
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // DELETE api/inschrijvingen/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInschrijving(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Niet geautoriseerd" });
                }

                var inschrijving = await _context.Inschrijvingen
                    .Include(i => i.Les)
                    .FirstOrDefaultAsync(i => i.Id == id && i.GebruikerId == userId);

                if (inschrijving == null)
                {
                    return NotFound(new { message = "Inschrijving niet gevonden" });
                }

                // Check cancellation policy (minstens 24 uur van tevoren)
                if (inschrijving.Les.StartTijd <= DateTime.Now.AddHours(24))
                {
                    return BadRequest(new
                    {
                        message = "Uitschrijven is alleen mogelijk tot 24 uur voor de les"
                    });
                }

                inschrijving.Status = "Geannuleerd";
                await _context.SaveChangesAsync();

                return Ok(new { message = "Succesvol uitgeschreven" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // Helper method for sorting
        private static System.Linq.Expressions.Expression<Func<Inschrijving, object>> GetSortExpression(string sort)
        {
            return sort.ToLower() switch
            {
                "gebruiker" => i => i.Gebruiker.Achternaam,
                "les" => i => i.Les.Naam,
                "datum" => i => i.InschrijfDatum,
                _ => i => i.InschrijfDatum
            };
        }
    }

    // DTO class
    public class CreateInschrijvingRequest
    {
        public int LesId { get; set; }
        public string Opmerkingen { get; set; } = string.Empty;
    }
}