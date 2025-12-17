using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbonnementenApiController : ControllerBase
    {
        private readonly IFitnessClubDbContext _context;

        public AbonnementenApiController(IFitnessClubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Abonnement>>> GetAbonnementen()
        {
            return await _context.Abonnementen.Where(a => a.IsActief).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Abonnement>> GetAbonnement(int id)
        {
            var abonnement = await _context.Abonnementen.FindAsync(id);
            if (abonnement == null || !abonnement.IsActief)
                return NotFound();
            return abonnement;
        }

        [HttpPost]
        public async Task<ActionResult<Abonnement>> PostAbonnement(Abonnement abonnement)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Abonnementen.Add(abonnement);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAbonnement), new { id = abonnement.Id }, abonnement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAbonnement(int id, Abonnement abonnement)
        {
            if (id != abonnement.Id)
                return BadRequest();
            if (!AbonnementExists(id))
                return NotFound();

           
            var entry = _context.Entry(abonnement);
            entry.State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbonnementExists(id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbonnement(int id)
        {
            var abonnement = await _context.Abonnementen.FindAsync(id);
            if (abonnement == null)
                return NotFound();

            abonnement.IsActief = false;

           
            var entry = _context.Entry(abonnement);
            entry.State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool AbonnementExists(int id)
        {
            return _context.Abonnementen.Any(e => e.Id == id);
        }
    }
}