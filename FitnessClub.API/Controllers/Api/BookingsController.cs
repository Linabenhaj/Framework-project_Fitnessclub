using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FitnessClub.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            FitnessClubDbContext context,
            ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetBookings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var bookings = await _context.Inschrijvingen
                .Include(b => b.Les)
                .Include(b => b.Gebruiker)
                .Where(b => b.GebruikerId == userId)
                .OrderByDescending(b => b.InschrijfDatum)
                .ToListAsync();

            return Ok(new { success = true, data = bookings });
        }

        // GET: api/bookings/all  (Admin: alle inschrijvingen)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllBookings()
        {
            var bookings = await _context.Inschrijvingen
                .Include(b => b.Les)
                .Include(b => b.Gebruiker)
                .OrderByDescending(b => b.InschrijfDatum)
                .ToListAsync();

            return Ok(new { success = true, count = bookings.Count, data = bookings });
        }

        // GET: api/bookings/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserBookings(string userId)
        {
            var bookings = await _context.Inschrijvingen
                .Include(b => b.Les)
                .Where(b => b.GebruikerId == userId)
                .ToListAsync();

            return Ok(new { success = true, data = bookings });
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<ActionResult<object>> PostBooking(BookingRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Controleer of activiteit bestaat
            var activity = await _context.Lessen.FindAsync(request.ActivityId);
            if (activity == null || !activity.IsActief)
            {
                return BadRequest(new { message = "Activiteit niet gevonden of niet actief" });
            }

            // Controleer of er nog plaats is
            var existingBookings = await _context.Inschrijvingen
                .CountAsync(b => b.LesId == request.ActivityId);

            if (existingBookings >= activity.MaxDeelnemers)
            {
                return BadRequest(new { message = "Geen plaatsen meer beschikbaar" });
            }

            var alreadyActief = await _context.Inschrijvingen
                .AnyAsync(b => b.LesId == request.ActivityId
                            && b.GebruikerId == userId
                            && b.Status == "Actief");

            if (alreadyActief)
            {
                return BadRequest(new { message = "Je hebt deze activiteit al geboekt" });
            }

            var oudGeannuleerd = await _context.Inschrijvingen
                .FirstOrDefaultAsync(b => b.LesId == request.ActivityId
                                       && b.GebruikerId == userId
                                       && b.Status == "Geannuleerd");
            if (oudGeannuleerd != null)
            {
                oudGeannuleerd.Status = "Actief";
                oudGeannuleerd.InschrijfDatum = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Re-booking: {userId} -> activiteit {request.ActivityId}");
                return Ok(new { success = true, message = "Boeking succesvol", bookingId = oudGeannuleerd.Id });
            }

            // Maak nieuwe booking
            var booking = new Inschrijving
            {
                LesId = request.ActivityId,
                GebruikerId = userId,
                InschrijfDatum = DateTime.Now,
                Status = "Actief"
            };

            _context.Inschrijvingen.Add(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Nieuwe booking: {userId} -> activiteit {request.ActivityId}");

            // Geen booking-object teruggeven 
            return Ok(new
            {
                success = true,
                message = "Boeking succesvol",
                bookingId = booking.Id
            });
        }

        // DELETE: api/bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var booking = await _context.Inschrijvingen.FindAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = "Boeking niet gevonden" });
            }

            // Controleer of booking van de gebruiker is
            if (booking.GebruikerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Update status
            booking.Status = "Geannuleerd";
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Booking geannuleerd" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetBooking(int id)
        {
            var booking = await _context.Inschrijvingen
                .Include(b => b.Les)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = "Boeking niet gevonden" });
            }

            return Ok(new { success = true, data = booking });
        }
    }

    public class BookingRequest
    {
        [Required]
        public int ActivityId { get; set; }

        public string Notes { get; set; }
    }
}