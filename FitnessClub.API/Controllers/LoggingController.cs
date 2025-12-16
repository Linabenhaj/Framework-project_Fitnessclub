using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;

        public LoggingController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // POST: api/logging/logerror
        [HttpPost("logerror")]
        public async Task<IActionResult> LogError([FromBody] LogError logError)
        {
            try
            {
                // Voeg timestamp toe als die ontbreekt
                if (logError.TimeStamp == DateTime.MinValue)
                    logError.TimeStamp = DateTime.UtcNow;

                _context.LogErrors.Add(logError);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, LogId = logError.Id });
            }
            catch (Exception ex)
            {
                // Fallback logging naar console
                Console.WriteLine($"Failed to save log: {ex.Message}");
                Console.WriteLine($"Log message was: {logError.Message}");

                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        // GET: api/logging/test
        [HttpGet("test")]
        public IActionResult TestLogging()
        {
            return Ok(new
            {
                Message = "Logging API is working",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}