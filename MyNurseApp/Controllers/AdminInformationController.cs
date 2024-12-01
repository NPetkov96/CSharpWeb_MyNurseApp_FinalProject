using Microsoft.AspNetCore.Mvc;

namespace MyNurseApp.Controllers
{
    public class AdminInformationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
