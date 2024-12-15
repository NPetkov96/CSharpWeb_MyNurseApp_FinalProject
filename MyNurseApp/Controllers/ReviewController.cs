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
        [Authorize]
        public IActionResult CreateReview()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(ReviewViewModel model)
        {
            try
            {
                await _reviewService.CreateReviewAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            try
            {
                await _reviewService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
