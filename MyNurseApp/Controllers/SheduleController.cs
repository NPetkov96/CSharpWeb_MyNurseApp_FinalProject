using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.PatientProfile;

namespace MyNurseApp.Controllers
{
    public class SheduleController : Controller
    {
        private readonly ScheduleService _scheduleService;
        public SheduleController(ScheduleService manipulationsService)
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
            return View(patientModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(HomeVisitationViewModel visitationModel)
        {
            await Task.CompletedTask;
            return View(visitationModel);
        }
    }
}
