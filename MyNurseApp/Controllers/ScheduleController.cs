using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.Manipulations;
using MyNurseApp.Web.ViewModels.PatientProfile;
using Newtonsoft.Json;

namespace MyNurseApp.Controllers
{
    [Authorize]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;
        private readonly INurseService _nurseService;
        public ScheduleController(IScheduleService scheduleService, INurseService nurseService)
        {
            _scheduleService = scheduleService;
            _nurseService = nurseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _scheduleService.GetVisitationsForUserAsync();
            if (model == null)
            {
                return RedirectToAction("CreatePatientProfile", "PatientProfile");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHomeVisitationFromPatient(Guid visitationId)
        {
            try
            {
                await _scheduleService.DeleteHomeVisitationAsync(visitationId);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex, nameof(Index));

            }
        }

        [HttpGet]
        public async Task<IActionResult> Schedule()
        {
            List<MedicalManipulationsViewModel> selectedManipulations = GetSelectedManipulations();
            PatientProfileViewModel patientModel = new PatientProfileViewModel();
            try
            {
                patientModel = await _scheduleService.GetPatientAync();
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex, "Error", "Home");
            }
            HomeVisitationViewModel homeVisitation = new HomeVisitationViewModel()
            {
                DateTimeManipulation = DateTime.Today
            };

            PatientAndHomeVisitationViewModel viewModel = new PatientAndHomeVisitationViewModel()
            {
                HomeVisitation = homeVisitation,
                PatientProfile = patientModel,
                MedicalManipulations = selectedManipulations!
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(PatientAndHomeVisitationViewModel model)
        {

            List<MedicalManipulationsViewModel> selectedManipulations = GetSelectedManipulations();
            PatientProfileViewModel patientModel = new PatientProfileViewModel();

            try
            {
                patientModel = await _scheduleService.GetPatientAync();
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex, "Error", "Home");
            }

            model.MedicalManipulations = selectedManipulations;
            model.PatientProfile = patientModel;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await _scheduleService.AddHomeVisitationAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex, nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllHomeVisitations()
        {
            var nurses = await _nurseService.GetAllNursesAsync();
            ViewData["Nurses"] = nurses;
            var viewModel = await _scheduleService.GetAllVisitationsAsync();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AssignVisitationToNurse(Guid visitationId, Guid nurseId)
        {
            try
            {
                await _scheduleService.AssignVisitationToNurseAsync(visitationId, nurseId);
                return RedirectToAction(nameof(GetAllHomeVisitations));
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex, nameof(GetAllHomeVisitations));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHomeVisitation(Guid visitationId)
        {
            try
            {
                await _scheduleService.DeleteHomeVisitationAsync(visitationId);
                return RedirectToAction(nameof(GetAllHomeVisitations));
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex, nameof(GetAllHomeVisitations));
            }
        }


        private List<MedicalManipulationsViewModel> GetSelectedManipulations()
        {
            var manipulationsJson = HttpContext.Session.GetString("SelectedManipulations");

            List<MedicalManipulationsViewModel>? selectedManipulations = manipulationsJson != null
                ? JsonConvert.DeserializeObject<List<MedicalManipulationsViewModel>>(manipulationsJson)
                : new List<MedicalManipulationsViewModel>();

            return selectedManipulations!;
        }
    }
}
