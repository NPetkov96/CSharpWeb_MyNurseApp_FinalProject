using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.Manipulations;
using MyNurseApp.Web.ViewModels.PatientProfile;
using Newtonsoft.Json;

namespace MyNurseApp.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ScheduleService _scheduleService;
        private readonly NurseService _nurseService;
        public ScheduleController(ScheduleService manipulationsService, NurseService nurseService)
        {
            _scheduleService = manipulationsService;
            _nurseService = nurseService;
        }

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
        public async Task<IActionResult> DeleteHomeVisitationFromPatient(Guid visitationId)
        {
            var isDeleted = await _scheduleService.DeleteHomeVisitationAsync(visitationId);
            if (!isDeleted)
            {
                return RedirectToAction("Index"); //TODO Handle exception
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Schedule()
        {
            List<MedicalManipulationsViewModel> selectedManipulations = GetSelectedManipulations();
            PatientProfileViewModel patientModel = await _scheduleService.GetPatientAync();
            HomeVisitationViewModel homeVisitation = new HomeVisitationViewModel();

            PatientAndHomeVisitationViewModel viewModel = new PatientAndHomeVisitationViewModel()
            {
                HomeVisitation = homeVisitation,
                PatientProfile = patientModel,
                MedicalManipulations = selectedManipulations!
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(PatientAndHomeVisitationViewModel model)
        {
            List<MedicalManipulationsViewModel> selectedManipulations = GetSelectedManipulations();
            PatientProfileViewModel patientModel = await _scheduleService.GetPatientAync();

            model.MedicalManipulations = selectedManipulations;
            model.PatientProfile = patientModel;

            await _scheduleService.AddHomeVisitationAsync(model);

            return RedirectToAction("Index");
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
        public async Task<IActionResult> AssignVisitationToNurse(Guid visitationId, Guid nurseId)
        {
            await _scheduleService.AssignVisitationToNurseAsync(visitationId, nurseId);
            return RedirectToAction("GetAllHomeVisitations");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHomeVisitation(Guid visitationId)
        {
            var isDeleted = await _scheduleService.DeleteHomeVisitationAsync(visitationId);
            if (!isDeleted)
            {
                return RedirectToAction("Index"); //TODO Handle exception
            }

            return RedirectToAction("GetAllHomeVisitations");
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
