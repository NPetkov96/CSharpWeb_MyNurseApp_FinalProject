using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository;
using MyNurseApp.Services.Data;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private ApplicationDbContext _context = null!;
        private BaseRepository<Review, Guid> _reviewRepository = null!;
        private ReviewService _reviewService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникална база за всеки тест
                .Options;

            _context = new ApplicationDbContext(options);
            _reviewRepository = new BaseRepository<Review, Guid>(_context);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            _reviewService = new ReviewService(_reviewRepository, httpContextAccessor.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllReviewsAsync_ReturnsAllReviews()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, UserName = "testUser" };
            _context.Users.Add(user);

            _context.Reviews.AddRange(
                new Review { Id = Guid.NewGuid(), Content = "Great service!", Rating = 5, UserId = userId, User = user },
                new Review { Id = Guid.NewGuid(), Content = "Could be better.", Rating = 3, UserId = userId, User = user }
            );
            await _context.SaveChangesAsync();

            // Act
            var reviews = await _reviewService.GetAllReviewsAsync();

            // Assert
            Assert.That(reviews, Is.Not.Null);
            Assert.That(reviews.Count(), Is.EqualTo(2));
            Assert.That(reviews.First().Content, Is.EqualTo("Great service!"));
        }

        [Test]
        public async Task DeleteAsync_RemovesReviewSuccessfully()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var review = new Review { Id = reviewId, Content = "Good", Rating = 4, UserId = Guid.NewGuid() };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            await _reviewService.DeleteAsync(reviewId);

            // Assert
            var deletedReview = await _context.Reviews.FindAsync(reviewId);
            Assert.That(deletedReview, Is.Null);
        }

        [Test]
        public void DeleteAsync_ThrowsException_WhenReviewNotFound()
        {
            // Arrange
            var reviewId = Guid.NewGuid();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _reviewService.DeleteAsync(reviewId));
        }

    }

}