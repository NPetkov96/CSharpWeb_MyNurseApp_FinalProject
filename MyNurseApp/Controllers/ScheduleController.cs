using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.PatientProfile;

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
            PatientProfileViewModel patientModel = await _scheduleService.GetPatient();
            HomeVisitationViewModel homeVisitation = new HomeVisitationViewModel();

            PatientAndHomeVisitationViewModel viewModel = new PatientAndHomeVisitationViewModel() 
            {
                HomeVisitation = homeVisitation,
                PatientProfile = patientModel
            };

            return View(viewModel);
        }
    }
}
