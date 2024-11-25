using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.Manipulations;

namespace MyNurseApp.Controllers
{
    [Authorize]
    public class ManipulationsController : Controller
    {
        private readonly ManipulationsService _manipulationsService;
        public ManipulationsController(ManipulationsService manipulationsService)
        {
            _manipulationsService = manipulationsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var manipulations = await _manipulationsService.GetAllManipulationsAsync();
            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View(manipulations);

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddManipulation()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddManipulation(MedicalManipulationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            await _manipulationsService.AddManipulationAsync(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveManipulation(Guid id)
        {
            await _manipulationsService.RemoveManipulationAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditManipulation(Guid id)
        {
            var model = await _manipulationsService.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditManipulation(MedicalManipulationsViewModel model)
        {
            await _manipulationsService.EditManipulationAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BookManipulation()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}
