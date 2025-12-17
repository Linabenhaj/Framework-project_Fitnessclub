using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace FitnessClub.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IFitnessClubDbContext _context;
        private readonly IHtmlLocalizer<AdminController> _localizer;

        public AdminController(
            IFitnessClubDbContext context,  
            IHtmlLocalizer<AdminController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = _localizer["AdminDashboard"];
            return View();
        }
    }
}