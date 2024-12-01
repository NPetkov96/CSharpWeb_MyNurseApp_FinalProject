using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;

namespace MyNurseApp.Controllers
{
    public class AdminInformationController : Controller
    {
        private readonly AdminInformationService _adminInformationService;

        public AdminInformationController(AdminInformationService adminInformationService)
        {
            this._adminInformationService = adminInformationService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _adminInformationService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var success = await _adminInformationService.DeleteUserAsync(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete user. Please try again.";
            }
            else
            {
                TempData["Success"] = "User deleted successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
