using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels.Manipulations;
using Newtonsoft.Json;


namespace MyNurseApp.Controllers
{
    [Authorize]
    public class ManipulationsController : Controller
    {
        private readonly IManipulationsService _manipulationsService;
        public ManipulationsController(IManipulationsService manipulationsService)
        {
            _manipulationsService = manipulationsService;
        }

        [HttpGet]
        public IActionResult SaveSelection()
        {
            var selectedManipulations = GetFromTempData<MedicalManipulationsViewModel>("SelectedManipulations", this);

            HttpContext.Session.SetString("SelectedManipulations", JsonConvert.SerializeObject(selectedManipulations));
            ClearTempData("SelectedManipulations", this);
            return RedirectToAction("Schedule", "Schedule");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 7)
        {
            var selectedManipulations = GetFromTempData<MedicalManipulationsViewModel>("SelectedManipulations", this);

            ViewBag.SelectedManipulations = selectedManipulations;

            var manipulations = await _manipulationsService.GetAllManipulationsAsync(pageNumber,pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_manipulationsService.GetTotalCount() / pageSize);
            return View(manipulations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToSelection(Guid id)
        {
            var manipulation = _manipulationsService.GetByIdAsync(id).Result;
            AddToTempData("SelectedManipulations", manipulation, this);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearSelection()
        {
            ClearTempData("SelectedManipulations", this);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddManipulation()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddManipulation(MedicalManipulationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _manipulationsService.AddManipulationAsync(model);
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
        public async Task<IActionResult> RemoveManipulation(Guid id)
        {
            try
            {
                await _manipulationsService.RemoveManipulationAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditManipulation(Guid id)
        {
            var model = await _manipulationsService.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditManipulation(MedicalManipulationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            await _manipulationsService.EditManipulationAsync(model);
            return RedirectToAction("Index");
        }


        public void AddToTempData<T>(string key, T item, Controller controller)
        {
            var existingData = controller.TempData[key] as string;
            var list = string.IsNullOrEmpty(existingData)
                ? new List<T>()
                : JsonConvert.DeserializeObject<List<T>>(existingData);

            list!.Add(item);

            controller.TempData[key] = JsonConvert.SerializeObject(list);
        }

        public List<T> GetFromTempData<T>(string key, Controller controller)
        {
            var data = controller.TempData.Peek(key) as string;
            return string.IsNullOrEmpty(data)
                ? new List<T>()
                : JsonConvert.DeserializeObject<List<T>>(data)!;
        }

        public void ClearTempData(string key, Controller controller)
        {
            controller.TempData[key] = null;
        }

        [HttpGet]
        public async Task<IActionResult> SearchMedicalManipulations(string query)
        {
            var manipulations = await _manipulationsService.SearchManipulationsAsync(query);
            return PartialView("_MedicalManipulationsList", manipulations);
        }
    }
}
