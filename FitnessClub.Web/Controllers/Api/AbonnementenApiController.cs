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
        private readonly FitnessClubDbContext _context;

        public AbonnementenApiController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET: api/AbonnementenApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Abonnement>>> GetAbonnementen()
        {
            return await _context.Abonnementen.Where(a => a.IsActief).ToListAsync();
        }

        // GET: api/AbonnementenApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Abonnement>> GetAbonnement(int id)
        {
            var abonnement = await _context.Abonnementen.FindAsync(id);

            if (abonnement == null || !abonnement.IsActief)
            {
                return NotFound();
            }

            return abonnement;
        }

        // POST: api/AbonnementenApi
        [HttpPost]
        public async Task<ActionResult<Abonnement>> PostAbonnement(Abonnement abonnement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Abonnementen.Add(abonnement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAbonnement), new { id = abonnement.Id }, abonnement);
        }

        // PUT: api/AbonnementenApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAbonnement(int id, Abonnement abonnement)
        {
            if (id != abonnement.Id)
            {
                return BadRequest();
            }

            if (!AbonnementExists(id))
            {
                return NotFound();
            }

            _context.Entry(abonnement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbonnementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/AbonnementenApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbonnement(int id)
        {
            var abonnement = await _context.Abonnementen.FindAsync(id);
            if (abonnement == null)
            {
                return NotFound();
            }

            abonnement.IsActief = false;
            _context.Entry(abonnement).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AbonnementExists(int id)
        {
            return _context.Abonnementen.Any(e => e.Id == id);
        }
    }
}