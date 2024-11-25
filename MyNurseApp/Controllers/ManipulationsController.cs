using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.Manipulations;


namespace MyNurseApp.Controllers
{
    [Authorize]
    public class ManipulationsController : Controller
    {
        private readonly ManipulationsService _manipulationsService;
        public ManipulationsController(ManipulationsService manipulationsService)
        {
            _manipulationsService = manipulationsService;
           
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Извличане на данните от TempData
            var selectedManipulations = GetFromTempData<MedicalManipulationsViewModel>("SelectedManipulations", this);

            ViewBag.SelectedManipulations = selectedManipulations;

            var manipulations = await _manipulationsService.GetAllManipulationsAsync();
            return View(manipulations);
        }

        [HttpPost]
        public async Task<IActionResult> AddToSelection(Guid id)
        {

            var manipulation = _manipulationsService.GetByIdAsync(id).Result;
            AddToTempData("SelectedManipulations", manipulation, this);
            await Task.CompletedTask;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearSelection()
        {
            ClearTempData("SelectedManipulations", this);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddManipulation()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddManipulation(MedicalManipulationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            await _manipulationsService.AddManipulationAsync(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveManipulation(Guid id)
        {
            await _manipulationsService.RemoveManipulationAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditManipulation(Guid id)
        {
            var model = await _manipulationsService.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditManipulation(MedicalManipulationsViewModel model)
        {
            await _manipulationsService.EditManipulationAsync(model);
            return RedirectToAction("Index");
        }


        public void AddToTempData<T>(string key, T item, Controller controller)
        {
            // Извличане на текущите данни от TempData
            var existingData = controller.TempData[key] as string;
            var list = string.IsNullOrEmpty(existingData)
                ? new List<T>() // Ако няма данни, създайте нов списък
                : Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(existingData);

            // Добавяне на новия елемент
            list!.Add(item);

            // Запазване обратно в TempData
            controller.TempData[key] = Newtonsoft.Json.JsonConvert.SerializeObject(list);
        }


        public List<T> GetFromTempData<T>(string key, Controller controller)
        {
            var data = controller.TempData.Peek(key) as string;
            return string.IsNullOrEmpty(data)
                ? new List<T>() // Ако няма данни, върнете празен списък
                : Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(data)!;
        }

        public void ClearTempData(string key, Controller controller)
        {
            controller.TempData[key] = null;
        }

    }
}
