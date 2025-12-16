using FitnessClub.Models.Data;      // Voor FitnessClubDbContext
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessClub.Web.Controllers
{using FitnessClub.Models.Data;
    public class AbonnementenController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
