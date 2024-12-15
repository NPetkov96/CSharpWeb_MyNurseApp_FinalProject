using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Controllers
{
    [Authorize]
    public class PatientProfileController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientProfileController(IPatientService patientService, IHttpContextAccessor httpContextAccessor)
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
                return RedirectToAction(nameof(CreatePatientProfile));
            }
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
        public IActionResult CreatePatientProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatientProfile(PatientProfileViewModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            try
            {
                await _patientService.AddPatientAsync(inputModel);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditPatientProfile(Guid id)
        {
            try
            {
                var profile = await _patientService.GetPatientProfileAync(id);
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
        public async Task<IActionResult> EditPatientProfile(PatientProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _patientService.EditPatientProfileAync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            try
            {
                await _patientService.DeletePatientAync(id);
                return RedirectToAction(nameof(GetAllPatientsProfiles));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
