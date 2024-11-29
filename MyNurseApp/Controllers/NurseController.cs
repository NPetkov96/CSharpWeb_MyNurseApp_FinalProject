using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Controllers
{
    public class NurseController : Controller
    {
        private readonly NurseService _nurseService;

        public NurseController(NurseService nurseService)
        {
            this._nurseService = nurseService;
        }


        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var viewModels = await _nurseService.GetNurseProfileAsync();
            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNurseProfile()
        {
            var viewModels = await _nurseService.GetNurseProfileAsync();
            if (viewModels != null)
            {
                return RedirectToAction("Profile", viewModels);
            }
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNurseProfile(NurseProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _nurseService.RegisterNurseAsync(model);

            return View("Index");
        }
    }
}
