using FitnessClub.Models.Data;     
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessClub.Web.Controllers
{
    public class TalenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
