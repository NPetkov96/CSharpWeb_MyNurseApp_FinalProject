using MyNurseApp.Common.Constants;
using MyNurseApp.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Web.ViewModels.Review
{
    public class ReviewViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please provide the content of the review.")]
        [StringLength(ReviewConstants.ContentMaxLength, ErrorMessage = "The content must be up to 100 characters.")]
        [Display(Name = "Review Content")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "Please provide a rating.")]
        [Range(ReviewConstants.RatingMinLength, ReviewConstants.RatingMaxLength, ErrorMessage = "The rating must be between 1 and 5.")]
        [Display(Name = "Rating")]
        public double Rating { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
