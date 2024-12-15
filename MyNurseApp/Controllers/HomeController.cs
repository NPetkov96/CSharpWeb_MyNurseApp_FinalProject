using Microsoft.AspNetCore.Mvc;

namespace MyNurseApp.Controllers
{
    public class HomeController : Controller
    {
       
        public HomeController()
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PendingApproval()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            var message = TempData["ErrorMessage"] as string ?? "An unexpected error occurred.";
            return View(model: message);
        }
    }
}
