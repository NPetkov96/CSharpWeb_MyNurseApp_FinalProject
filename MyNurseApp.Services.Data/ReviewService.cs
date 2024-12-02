using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Web.ViewModels.Review;
using System.Security.Claims;

namespace MyNurseApp.Services.Data
{
    public class ReviewService
    {
        private readonly IRepository<Review, Guid> _reviewRepository;
        private readonly IHttpContextAccessor _currentAccsessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewService(IRepository<Review, Guid> reviewRepository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this._reviewRepository = reviewRepository;
            this._currentAccsessor = httpContextAccessor;
            this._userManager = userManager;
        }

        public async Task<IEnumerable<ReviewViewModel>> GetAllReviewsAsync()
        {
            var models = await _reviewRepository
                    .GetAllAttached()
                    .Include(r => r.User)
                    .ToListAsync();

            var viewModels = models.Select(model => ConvertToViewModel(model)).ToList();

            return viewModels;
        }

        public async Task<bool> CreateReviewAsync(ReviewViewModel viewModel)
        {
            var model = ConvertToModel(viewModel);

            await _reviewRepository.AddAsync(model);

            if (await _reviewRepository.GetByIdAsync(model.Id) == null)
            {
                return false;
            }

            return true;
        }


        private ReviewViewModel ConvertToViewModel(Review model)
        {
            var viewModel = new ReviewViewModel()
            {
                Id = model.Id,
                Content = model.Content,
                Rating = model.Rating,
                UserId = model.UserId,
                User = model.User
            };

            return viewModel;
        }

        private Review ConvertToModel(ReviewViewModel viewModel)
        {
            var userId = _currentAccsessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var model = new Review()
            {
                Id = Guid.NewGuid(),
                Content = viewModel.Content,
                Rating = viewModel.Rating,
                UserId = Guid.Parse(userId!)
            };

            return model;
        }

        public async Task<ReviewViewModel> GetByIdAsync(Guid id)
        {
            var review = await _reviewRepository.FirstOrDefaultAsync(r => r.Id == id);
            var currentUserId = _currentAccsessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _currentAccsessor.HttpContext?.User?.IsInRole("Admin") ?? false;
            if (review.UserId.ToString() != currentUserId && !isAdmin)
            {
                throw new InvalidOperationException("You do not have access to this review.");
            }
            return ConvertToViewModel(review);
        }



        public async Task DeleteAsync(Guid id)
        {
            var review = await _reviewRepository.FirstOrDefaultAsync(r => r.Id == id);
            var model = new Review()
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                UserId = review.UserId
            };

            await _reviewRepository.DeleteAsync(model);

        }
    }
}
