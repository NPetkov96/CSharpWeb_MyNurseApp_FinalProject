using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data.Interfaces;
using MyNurseApp.Web.ViewModels.Review;
using System.Security.Claims;

namespace MyNurseApp.Services.Data
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review, Guid> _reviewRepository;
        private readonly IHttpContextAccessor _currentAccsessor;

        public ReviewService(IRepository<Review, Guid> reviewRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._reviewRepository = reviewRepository;
            this._currentAccsessor = httpContextAccessor;
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

        public async Task CreateReviewAsync(ReviewViewModel viewModel)
        {
            var model = ConvertToModel(viewModel);

            await _reviewRepository.AddAsync(model);

            if (!_reviewRepository.GetAllAttached().Any(m=>m.Id == model.Id))
            {
                throw new InvalidOperationException("Review could not be added.");
            }
        }
        
        public async Task DeleteAsync(Guid id)
        {
            var review = await _reviewRepository.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null)
            {
                throw new InvalidOperationException("Review could not be found.");
            }
            bool isDeleted = await _reviewRepository.DeleteAsync(review);
            if (!isDeleted)
            {
                throw new InvalidOperationException("Review could not be deleted.");
            }
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

    }
}
