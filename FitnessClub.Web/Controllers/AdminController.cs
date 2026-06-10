using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IFitnessClubDbContext _context;
        private readonly UserManager<Gebruiker> _userManager;

        public AdminController(
            IFitnessClubDbContext context,
            UserManager<Gebruiker> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin/Dashboard
        // Hoofd-overzicht voor de beheerder met statistieken en snelle acties
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Admin Dashboard";

            // Asynchroon ophalen van alle counts
            var totaalLeden = await _userManager.GetUsersInRoleAsync("Lid");
            var aantalLeden = totaalLeden.Count;

            var aantalLessen = await _context.Lessen.CountAsync(l => l.IsActief);
            var aantalAbonnementen = await _context.Abonnementen.CountAsync();
            var aantalInschrijvingen = await _context.Inschrijvingen.CountAsync();

            // Volgende 5 lessen
            var aankomendeLessen = await _context.Lessen
                .Where(l => l.IsActief && l.StartTijd >= DateTime.Now)
                .OrderBy(l => l.StartTijd)
                .Take(5)
                .ToListAsync();

            // Recente leden (laatste 5)
            var recenteLeden = totaalLeden
                .OrderByDescending(u => u.AangemaaktOp)
                .Take(5)
                .ToList();

            ViewBag.AantalLeden = aantalLeden;
            ViewBag.AantalLessen = aantalLessen;
            ViewBag.AantalAbonnementen = aantalAbonnementen;
            ViewBag.AantalInschrijvingen = aantalInschrijvingen;
            ViewBag.AankomendeLessen = aankomendeLessen;
            ViewBag.RecenteLeden = recenteLeden;

            return View();
        }
    }
}
