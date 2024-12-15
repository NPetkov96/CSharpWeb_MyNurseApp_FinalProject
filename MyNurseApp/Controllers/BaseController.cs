using Microsoft.AspNetCore.Mvc;

namespace MyNurseApp.Controllers
{
    public class BaseController : Controller
    {
        protected IActionResult HandleError(Exception ex, string redirectAction, string redirectController = "Home")
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(redirectAction, redirectController);
        }
    }
}
