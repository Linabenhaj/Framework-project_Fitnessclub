using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;




namespace FitnessClub.Web.Controllers
{
    public class LessenController : Controller
    {
        private readonly FitnessClubDbContext _context;

        public LessenController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET: Lessen
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NaamSortParm"] = String.IsNullOrEmpty(sortOrder) ? "naam_desc" : "";
            ViewData["StartTijdSortParm"] = sortOrder == "StartTijd" ? "start_desc" : "StartTijd";
            ViewData["TrainerSortParm"] = sortOrder == "Trainer" ? "trainer_desc" : "Trainer";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var lessen = from l in _context.Lessen
                         where !l.IsVerwijderd
                         select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                lessen = lessen.Where(l => l.Naam.Contains(searchString)
                                   || l.Beschrijving.Contains(searchString)
                                   || l.Trainer.Contains(searchString)
                                   || l.Locatie.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "naam_desc":
                    lessen = lessen.OrderByDescending(l => l.Naam);
                    break;
                case "StartTijd":
                    lessen = lessen.OrderBy(l => l.StartTijd);
                    break;
                case "start_desc":
                    lessen = lessen.OrderByDescending(l => l.StartTijd);
                    break;
                case "Trainer":
                    lessen = lessen.OrderBy(l => l.Trainer);
                    break;
                case "trainer_desc":
                    lessen = lessen.OrderByDescending(l => l.Trainer);
                    break;
                default:
                    lessen = lessen.OrderBy(l => l.StartTijd);
                    break;
            }

            int pageSize = 10;

            // GECORRIGEERDE REGEL 73:
            return View(await FitnessClub.Web.PaginatedList<Les>.CreateAsync(lessen.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Lessen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var les = await _context.Lessen
                .Include(l => l.Inschrijvingen)
                    .ThenInclude(i => i.Gebruiker)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsVerwijderd);

            if (les == null)
            {
                return NotFound();
            }

            return View(les);
        }

        // GET: Lessen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lessen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Naam,Beschrijving,StartTijd,EindTijd,MaxDeelnemers,Locatie,Trainer,IsActief")] Les les)
        {
            if (ModelState.IsValid)
            {
                les.AangemaaktOp = DateTime.UtcNow;
                _context.Add(les);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(les);
        }

        // GET: Lessen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var les = await _context.Lessen.FindAsync(id);
            if (les == null || les.IsVerwijderd)
            {
                return NotFound();
            }
            return View(les);
        }

        // POST: Lessen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naam,Beschrijving,StartTijd,EindTijd,MaxDeelnemers,Locatie,Trainer,IsActief,AangemaaktOp")] Les les)
        {
            if (id != les.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    les.GewijzigdOp = DateTime.UtcNow;
                    _context.Update(les);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LesExists(les.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(les);
        }

        // GET Lessen/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var les = await _context.Lessen
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsVerwijderd);

            if (les == null)
            {
                return NotFound();
            }

            return View(les);
        }

        // POST Lessen/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var les = await _context.Lessen.FindAsync(id);
            if (les != null)
            {
                les.IsVerwijderd = true;
                les.VerwijderdOp = DateTime.UtcNow;
                _context.Lessen.Update(les);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



        private bool LesExists(int id)
        {
            return _context.Lessen.Any(e => e.Id == id && !e.IsVerwijderd);
        }
    }
}
