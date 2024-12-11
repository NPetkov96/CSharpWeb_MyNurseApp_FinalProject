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
        private Mock<IRepository<HomeVisitation, Guid>> _mockVisitationRepository = null!;
        private Mock<IRepository<PatientProfile, Guid>> _mockPatientRepository = null!;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor = null!;
        private ScheduleService _scheduleService = null!;

        [SetUp]
        public void Setup()
        {
            _mockVisitationRepository = new Mock<IRepository<HomeVisitation, Guid>>();
            _mockPatientRepository = new Mock<IRepository<PatientProfile, Guid>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _scheduleService = new ScheduleService(
                null!, 
                _mockVisitationRepository.Object,
                _mockPatientRepository.Object,
                _mockHttpContextAccessor.Object,
                null!
            );
        }

        [Test]
        public void GetVisitationsForUserAsync_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null!);

            Assert.That(async () => await _scheduleService.GetVisitationsForUserAsync(),
                Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo("User is not authenticated."));
        }

        [Test]
        public async Task GetVisitationsForUserAsync_ReturnsNull_WhenPatientDoesNotExist()
        {
            var userId = Guid.NewGuid().ToString();
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            _mockPatientRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<PatientProfile, bool>>>()))
                .ReturnsAsync((PatientProfile)null!);

            var result = await _scheduleService.GetVisitationsForUserAsync();

            Assert.That(result, Is.Null);
        }
    }
}