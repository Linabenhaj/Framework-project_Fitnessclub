using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FitnessClub.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikersApiController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;
        private readonly UserManager<Gebruiker> _userManager;
        private readonly IConfiguration _configuration;

        public GebruikersApiController(
            FitnessClubDbContext context,
            UserManager<Gebruiker> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET api/gebruikers
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetGebruikers()
        {
            var gebruikers = await _context.Users
                .Include(u => u.Abonnement)
                .Include(u => u.Inschrijvingen)
                .ThenInclude(i => i.Les)
                .Where(u => !u.IsVerwijderd)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Voornaam,
                    u.Achternaam,
                    Telefoon = u.PhoneNumber, // Gebruik PhoneNumber i.p.v. Telefoon
                    u.Geboortedatum,
                    u.Rol,
                    Abonnement = u.Abonnement != null ? new
                    {
                        u.Abonnement.Id,
                        u.Abonnement.Naam,
                        u.Abonnement.Prijs
                    } : null,
                    Inschrijvingen = u.Inschrijvingen.Select(i => new
                    {
                        i.Id,
                        LesNaam = i.Les.Naam,
                        i.Les.StartTijd,
                        i.Status
                    })
                })
                .ToListAsync();

            return Ok(gebruikers);
        }

        // GET api/gebruikers/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<object>> GetGebruiker(string id)
        {
            var gebruiker = await _context.Users
                .Include(u => u.Abonnement)
                .Include(u => u.Inschrijvingen)
                .ThenInclude(i => i.Les)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsVerwijderd);

            if (gebruiker == null)
            {
                return NotFound(new { message = "Gebruiker niet gevonden" });
            }

            return Ok(new
            {
                gebruiker.Id,
                gebruiker.Email,
                gebruiker.Voornaam,
                gebruiker.Achternaam,
                Telefoon = gebruiker.PhoneNumber, // Gebruik PhoneNumber i.p.v. Telefoon
                gebruiker.Geboortedatum,
                gebruiker.Rol,
                Abonnement = gebruiker.Abonnement != null ? new
                {
                    gebruiker.Abonnement.Id,
                    gebruiker.Abonnement.Naam,
                    gebruiker.Abonnement.Prijs
                } : null,
                Inschrijvingen = gebruiker.Inschrijvingen.Select(i => new
                {
                    i.Id,
                    LesNaam = i.Les.Naam,
                    i.Les.StartTijd,
                    i.Status
                })
            });
        }

        // POST api/gebruikers/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email en wachtwoord zijn verplicht" });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Ongeldige inloggegevens" });
            }

            if (user.IsVerwijderd)
            {
                return Unauthorized(new { message = "Account is gedeactiveerd" });
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return Unauthorized(new { message = "Ongeldige inloggegevens" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Voornaam,
                user.Achternaam,
                Telefoon = user.PhoneNumber, // Gebruik PhoneNumber i.p.v. Telefoon
                Roles = roles,
                Token = token,
                Expires = DateTime.UtcNow.AddHours(3)
            });
        }

        // POST api/gebruikers/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return BadRequest(new { message = "Email is al in gebruik" });
            }

            var user = new Gebruiker
            {
                UserName = request.Email,
                Email = request.Email,
                Voornaam = request.Voornaam,
                Achternaam = request.Achternaam,
                PhoneNumber = request.Telefoon, // Gebruik PhoneNumber
                Geboortedatum = request.Geboortedatum ?? DateTime.Now.AddYears(-20),
                Rol = "Lid",
                AangemaaktOp = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Registratie mislukt",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            await _userManager.AddToRoleAsync(user, "Lid");

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Voornaam,
                user.Achternaam,
                Telefoon = user.PhoneNumber, // Gebruik PhoneNumber
                Token = token,
                Message = "Registratie succesvol"
            });
        }

        private string GenerateJwtToken(Gebruiker user, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "DefaultKeyForDevelopment1234567890123456"));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("voornaam", user.Voornaam),
                new Claim("achternaam", user.Achternaam)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "FitnessClubAPI",
                audience: _configuration["Jwt:Audience"] ?? "FitnessClubUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO klassen
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public string Telefoon { get; set; } = string.Empty;
        public DateTime? Geboortedatum { get; set; }
    }
}