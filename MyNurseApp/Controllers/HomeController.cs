using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Models;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.PatientProfile;
using System.Diagnostics;

namespace MyNurseApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, PatientService patientService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _patientService = patientService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask; 
            return View();
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
        public async Task<IActionResult> CreatePatientProfile(PatientProfileinputModel inputModel)
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
