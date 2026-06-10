using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GebruikersController : ControllerBase
    {
        private readonly UserManager<Gebruiker> _userManager;
        private readonly ILogger<GebruikersController> _logger;

        public GebruikersController(
            UserManager<Gebruiker> userManager,
            ILogger<GebruikersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGebruikers()
        {
            _logger.LogInformation("Admin vraagt gebruikerslijst op");

            var gebruikers = await _userManager.Users.ToListAsync();

            var result = new List<object>();
            foreach (var gebruiker in gebruikers)
            {
                var roles = await _userManager.GetRolesAsync(gebruiker);
                result.Add(new
                {
                    id = gebruiker.Id,
                    email = gebruiker.Email,
                    voornaam = gebruiker.FirstName,
                    achternaam = gebruiker.LastName,
                    rol = roles.FirstOrDefault() ?? "Lid"
                });
            }

            return Ok(new
            {
                success = true,
                count = result.Count,
                data = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGebruiker(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "Gebruiker niet gevonden" });

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                _logger.LogWarning($"Poging om admin-account {user.Email} te verwijderen — geweigerd");
                return BadRequest(new { success = false, message = "Een admin-account kan niet verwijderd worden" });
            }
            if (roles.Contains("Trainer"))
            {
                _logger.LogWarning($"Poging om trainer-account {user.Email} te verwijderen — geweigerd");
                return BadRequest(new { success = false, message = "Een trainer-account kan niet verwijderd worden" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
            }

            _logger.LogInformation($"Gebruiker {user.Email} verwijderd door admin");
            return Ok(new { success = true, message = "Gebruiker verwijderd" });
        }
    }
}
