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
        private readonly PatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientProfileController(PatientService patientService, IHttpContextAccessor httpContextAccessor)
        {
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPatientsProfiles()
        {
            var viewModels = await _patientService.GetAllPatientsAsync();
            return View(viewModels);
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

            await _patientService.AddPatientAsync(inputModel);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditPatientProfile(Guid id)
        {
            var profile = await _patientService.GetPatientProfileAync(id);
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> EditPatientProfile(PatientProfileViewModel model)
        {
            await _patientService.EditPatientProfileAync(model);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            await _patientService.DeletePatientAync(id);
            await Task.CompletedTask;
            return RedirectToAction("GetAllPatientsProfiles");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            await Task.CompletedTask;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
