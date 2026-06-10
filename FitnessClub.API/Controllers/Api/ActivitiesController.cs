using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivitiesController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly ILogger<ActivitiesController> _logger;

        public ActivitiesController(
            FitnessClubDbContext context,
            ILogger<ActivitiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetActivities(
            [FromQuery] string search = "",
            [FromQuery] string type = "",
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string sortBy = "date")
        {
            _logger.LogInformation("Ophalen activiteiten");

            var query = _context.Lessen.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a =>
                    a.Naam.Contains(search) ||
                    a.Beschrijving.Contains(search) ||
                    a.Trainer.Contains(search));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(a => a.Type == type);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.StartTijd >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.StartTijd <= toDate.Value);
            }

            query = sortBy.ToLower() switch
            {
                "name" => query.OrderBy(a => a.Naam),
                "name_desc" => query.OrderByDescending(a => a.Naam),
                "date_desc" => query.OrderByDescending(a => a.StartTijd),
                "trainer" => query.OrderBy(a => a.Trainer),
                _ => query.OrderBy(a => a.StartTijd)
            };

            var activities = await query.Where(a => a.IsActief).ToListAsync();

            return Ok(new
            {
                success = true,
                count = activities.Count,
                data = activities,
                timestamp = DateTime.Now
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetActivity(int id)
        {
            var activity = await _context.Lessen.FindAsync(id);

            if (activity == null)
            {
                return NotFound(new { message = $"Activiteit met ID {id} niet gevonden" });
            }

            return Ok(new { success = true, data = activity });
        }

        [HttpGet("today")]
        public async Task<ActionResult> GetTodayActivities()
        {
            var today = DateTime.Today;
            var activities = await _context.Lessen
                .Where(a => a.StartTijd.Date == today && a.IsActief)
                .OrderBy(a => a.StartTijd)
                .ToListAsync();

            return Ok(activities);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<ActionResult> PostActivity(Les activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Lessen.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity),
                new { id = activity.Id },
                new { success = true, data = activity });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> PutActivity(int id, Les activity)
        {
            if (id != activity.Id)
            {
                return BadRequest();
            }

            _context.Entry(activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(new { success = true, message = "Activiteit bijgewerkt" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Lessen.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            activity.IsActief = false;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Activiteit gedeactiveerd" });
        }

        private bool ActivityExists(int id)
        {
            return _context.Lessen.Any(e => e.Id == id);
        }
    }
}