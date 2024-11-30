using Microsoft.AspNetCore.Mvc;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.Review;

namespace MyNurseApp.Controllers
{
    public class ReviewController : Controller
    {

        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewViewModel model)
        {
            var isCreated = await _reviewService.CreateReviewAsync(model);
            if (!isCreated) 
            {
                throw new ArgumentException("Couldn't create review"); //TODO better handling
            }
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> DeleteReview(Guid id)
        //{
        //    //var review = await _reviewService.GetByIdAsync(id);

        //    //if (review == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //// Проверка дали потребителят има право да изтрие ревюто
        //    //var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    //if (review.UserId.ToString() != currentUserId && !User.IsInRole("Admin"))
        //    //{
        //    //    return Forbid();
        //    //}

        //    //await _reviewService.DeleteAsync(review);

        //    //return RedirectToAction("Index");
        //}
    }
}
