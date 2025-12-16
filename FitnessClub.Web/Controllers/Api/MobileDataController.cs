using FitnessClub.Models.Data;  
using Microsoft.AspNetCore.Mvc;

namespace FitnessClub.Web.Controllers.Api
{
    public class MobileDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
