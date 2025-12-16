using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AbonnementenController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;

        public AbonnementenController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET: api/abonnementen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Abonnement>>> GetAbonnementen()
        {
            var abonnementen = await _context.Abonnementen
                .Where(a => a.IsActief)
                .OrderBy(a => a.Prijs)
                .ToListAsync();

            return Ok(abonnementen);
        }
    }
}