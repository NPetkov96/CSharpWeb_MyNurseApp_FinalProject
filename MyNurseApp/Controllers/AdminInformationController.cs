using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data.Interfaces;

namespace MyNurseApp.Controllers
{
    [Authorize]
    public class AdminInformationController : BaseController
    {
        private readonly IAdminInformationService _adminInformationService;

        public AdminInformationController(IAdminInformationService adminInformationService)
        {
            this._adminInformationService = adminInformationService;
        }

        [HttpGet]
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
            return RedirectToAction(nameof(Index));
        }
    }
}
