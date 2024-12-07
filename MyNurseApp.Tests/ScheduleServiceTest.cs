using Microsoft.AspNetCore.Http;
using Moq;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class ScheduleServiceTest
    {
        private Mock<IRepository<HomeVisitation, Guid>> _mockVisitationRepository;
        private Mock<IRepository<PatientProfile, Guid>> _mockPatientRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private ScheduleService _scheduleService;

        [SetUp]
        public void Setup()
        {
            _mockVisitationRepository = new Mock<IRepository<HomeVisitation, Guid>>();
            _mockPatientRepository = new Mock<IRepository<PatientProfile, Guid>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _scheduleService = new ScheduleService(
                null, // manipulationRepository (не е използван тук)
                _mockVisitationRepository.Object,
                _mockPatientRepository.Object,
                _mockHttpContextAccessor.Object,
                null // nurseRepository (не е използван тук)
            );
        }

        [Test]
        public void GetVisitationsForUserAsync_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null);

            // Act & Assert
            Assert.That(async () => await _scheduleService.GetVisitationsForUserAsync(),
                Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo("User is not authenticated."));
        }

        [Test]
        public async Task GetVisitationsForUserAsync_ReturnsNull_WhenPatientDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            _mockPatientRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<PatientProfile, bool>>>()))
                .ReturnsAsync((PatientProfile)null);

            // Act
            var result = await _scheduleService.GetVisitationsForUserAsync();

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}