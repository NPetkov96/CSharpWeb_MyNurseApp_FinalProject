using MyNurseApp.Web.ViewModels.Review;

namespace MyNurseApp.Services.Data.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewViewModel>> GetAllReviewsAsync();

        Task CreateReviewAsync(ReviewViewModel viewModel);

        Task DeleteAsync(Guid id);
    }
}
