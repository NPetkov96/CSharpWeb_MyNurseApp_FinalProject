using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Controllers
{
    [Authorize]
    public class NurseController : Controller
    {
        private readonly INurseService _nurseService;

        public NurseController(INurseService nurseService)
        {
            this._nurseService = nurseService;
        }

        public async Task<IActionResult> Profile()
        {
            try
            {
                var viewModels = await _nurseService.GetNurseProfileAsync();
                return View(viewModels);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditNurseProfile()
        {
            try
            {
                var profile = await _nurseService.GetNurseProfileAsync();
                return View(profile);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNurseProfile(NurseProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await _nurseService.EditNurseProfileAync(model);
                return RedirectToAction("Profile");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> RequestedVisitations()
        {
            var viewModels = await _nurseService.GetNurseHomeVisitatonsAync();
            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> FinishVisitaion(Guid id)
        {
            try
            {
                await _nurseService.GetNurseHomeVisitatonsAync(id);
                return RedirectToAction(nameof(RequestedVisitations));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> CreateNurseProfile()
        {
            var viewModels = await _nurseService.GetNurseProfileAsync();
            if (viewModels != null)
            {
                return RedirectToAction(nameof(Profile), viewModels);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNurseProfile(NurseProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _nurseService.RegisterNurseAsync(model);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllNursesProfiles()
        {
            var viewModels = await _nurseService.GetAllNursesAsync();
            return View(viewModels);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprooveNurse(Guid id)
        {
            await _nurseService.AprooveNurseAync(id);
            return RedirectToAction(nameof(GetAllNursesProfiles));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineNurse(Guid id)
        {
            await _nurseService.DeclineNurseAync(id);
            return RedirectToAction(nameof(GetAllNursesProfiles));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNurseProfile(Guid id)
        {
            try
            {
                await _nurseService.DeleteNurseProfileAync(id);
                return RedirectToAction(nameof(GetAllNursesProfiles));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
