using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Controllers
{
    public class NurseController : Controller
    {
        private readonly NurseService _nurseService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NurseController(NurseService nurseService, IHttpContextAccessor httpContextAccessor)
        {
            this._nurseService = nurseService;
            this._httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> Index()
        {
            var viewModels = await _nurseService.GetAllNursesAsync();
            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNurseProfile()
        {
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
