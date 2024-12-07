using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.Review;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class ReviewServiceTest
    {
        private Mock<IRepository<Review, Guid>> _mockReviewRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private ReviewService _reviewService;

        [SetUp]
        public void Setup()
        {
            _mockReviewRepository = new Mock<IRepository<Review, Guid>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _reviewService = new ReviewService(
                _mockReviewRepository.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Test]
        public async Task GetAllReviewsAsync_ReturnsAllReviews()
        {
            // Arrange
            var reviews = new List<Review>
        {
            new Review { Id = Guid.NewGuid(), Content = "Great service!", Rating = 5, UserId = Guid.NewGuid() },
            new Review { Id = Guid.NewGuid(), Content = "Could be better.", Rating = 3, UserId = Guid.NewGuid() }
        }.AsQueryable();

            var mockDbSet = CreateMockAsyncDbSet(reviews);

            _mockReviewRepository.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            // Act
            var result = await _reviewService.GetAllReviewsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
            Assert.That(result.First().Content, Is.EqualTo("Great service!"));
        }

        [Test]
        public async Task DeleteAsync_RemovesReviewSuccessfully()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var review = new Review { Id = reviewId, Content = "Good", Rating = 4 };

            _mockReviewRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync(review);

            _mockReviewRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Review>()))
                .ReturnsAsync(true);

            // Act
            await _reviewService.DeleteAsync(reviewId);

            // Assert
            _mockReviewRepository.Verify(repo => repo.DeleteAsync(It.Is<Review>(r => r.Id == reviewId)), Times.Once);
        }

        [Test]
        public void DeleteAsync_ThrowsException_WhenReviewNotFound()
        {
            // Arrange
            _mockReviewRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync((Review)null);

            var reviewId = Guid.NewGuid();

            // Act & Assert
            Assert.That(async () => await _reviewService.DeleteAsync(reviewId),
                Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Review could not be found."));
        }

        // Helper method to mock DbSet with async support
        private static Mock<DbSet<T>> CreateMockAsyncDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            return mockSet;
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

            public T Current => _inner.Current;
        }
    }
}