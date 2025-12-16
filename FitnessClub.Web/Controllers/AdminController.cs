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
        private readonly IHtmlLocalizer<AdminController> _localizer;

        public AdminController(IHtmlLocalizer<AdminController> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = _localizer["AdminDashboard"];
            return View();
        }
    }
}