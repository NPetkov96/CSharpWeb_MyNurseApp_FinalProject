using Microsoft.AspNetCore.Mvc;

namespace MyNurseApp.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
