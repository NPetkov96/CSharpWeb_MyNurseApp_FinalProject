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
        public ScheduleController(ScheduleService manipulationsService)
        {
            _scheduleService = manipulationsService;
        }

        public async Task<IActionResult> Index()
        {
           var model = await _scheduleService.GetAllVisitationsAsync();
            return View(model);
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

        private List<MedicalManipulationsViewModel> GetSelectedManipulations()
        {
            // Вземане на манипулациите от сесията
            var manipulationsJson = HttpContext.Session.GetString("SelectedManipulations");

            List<MedicalManipulationsViewModel>? selectedManipulations = manipulationsJson != null
                ? JsonConvert.DeserializeObject<List<MedicalManipulationsViewModel>>(manipulationsJson)
                : new List<MedicalManipulationsViewModel>();

            return selectedManipulations!;
        }
    }
}
