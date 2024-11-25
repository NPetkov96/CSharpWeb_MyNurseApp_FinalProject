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
            await Task.CompletedTask;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Schedule()
        {
            // Вземане на манипулациите от сесията
            var manipulationsJson = HttpContext.Session.GetString("SelectedManipulations");
            List<MedicalManipulationsViewModel> selectedManipulations = manipulationsJson != null
                ? JsonConvert.DeserializeObject<List<MedicalManipulationsViewModel>>(manipulationsJson)
                : new List<MedicalManipulationsViewModel>();

            PatientProfileViewModel patientModel = await _scheduleService.GetPatient();
            HomeVisitationViewModel homeVisitation = new HomeVisitationViewModel();
           
            PatientAndHomeVisitationViewModel viewModel = new PatientAndHomeVisitationViewModel()
            {
                HomeVisitation = homeVisitation,
                PatientProfile = patientModel,
                MedicalManipulations = selectedManipulations
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Schedule(PatientAndHomeVisitationViewModel model)
        {
            //Add visitation do DB
            return RedirectToAction("Index");
        }
    }
}
