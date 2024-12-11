using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels.Review;

namespace MyNurseApp.Controllers
{
    public class ReviewController : Controller
    {

        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this._reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var models = await _reviewService.GetAllReviewsAsync();
            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> CreateReview()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewViewModel model)
        {
            try
            {
                await _reviewService.CreateReviewAsync(model);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            try
            {
                await _reviewService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
