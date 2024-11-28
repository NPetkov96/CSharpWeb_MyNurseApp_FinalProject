using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Models;
using System.Diagnostics;

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
        public IActionResult PendingApproval()
        {
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
            await Task.CompletedTask; 
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
