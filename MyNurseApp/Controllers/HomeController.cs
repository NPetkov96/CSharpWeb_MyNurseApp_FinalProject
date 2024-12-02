using Microsoft.AspNetCore.Mvc;

namespace MyNurseApp.Controllers
{
    public class HomeController : Controller
    {
       
        public HomeController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask; 
            return View();
        }
        public async Task<IActionResult> PendingApproval()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            await Task.CompletedTask;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            var message = TempData["ErrorMessage"] as string ?? "An unexpected error occurred.";
            await Task.CompletedTask;
            return View(model: message);
        }
    }
}
