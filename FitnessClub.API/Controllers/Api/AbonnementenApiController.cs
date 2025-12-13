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
                    AantalGebruikers = a.Gebruikers.Count
                })
                .ToListAsync();

            return Ok(abonnementen);
        }


      
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAbonnement(int id)
        {
            var abonnement = await _context.Abonnementen
                .Include(a => a.Gebruikers)
                .FirstOrDefaultAsync(a => a.Id == id);



            if (abonnement == null)
            {
                return NotFound(new { message = "Abonnement niet gevonden" });
            }



            return Ok(new
            {
                abonnement.Id,
                abonnement.Naam,
                abonnement.Prijs,
                abonnement.Omschrijving,
                abonnement.LooptijdMaanden,
                Gebruikers = abonnement.Gebruikers.Select(g => new
                {
                    g.Id,
                    Naam = g.Voornaam + " " + g.Achternaam,
                    g.Email
                })
            });
        }
    }
}