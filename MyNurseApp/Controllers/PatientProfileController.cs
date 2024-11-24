using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Models;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.PatientProfile;
using System.Diagnostics;

namespace MyNurseApp.Controllers
{
    [Authorize]
    public class PatientProfileController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientProfileController(ILogger<HomeController> logger, PatientService patientService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _patientService = patientService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            PatientProfileViewModel patientProfile = await _patientService.GetPatientProfileByUserIdAsync(_httpContextAccessor);

            if (patientProfile == null)
            {
                return RedirectToAction("CreatePatientProfile");
            }
            await Task.CompletedTask;
            return View(patientProfile);
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreatePatientProfile()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatientProfile(PatientProfileViewModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await _patientService.AddPatientAsync(inputModel, _httpContextAccessor);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            await Task.CompletedTask;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
