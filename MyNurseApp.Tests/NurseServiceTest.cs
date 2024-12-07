using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Data;
using MyNurseApp.Services.Data;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class NurseServiceTest
    {
        private Mock<IRepository<NurseProfile, Guid>> _mockNurseRepository;
        private Mock<IRepository<HomeVisitation, Guid>> _mockVisitationRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private NurseService _nurseService;

        [SetUp]
        public void Setup()
        {
            _mockNurseRepository = new Mock<IRepository<NurseProfile, Guid>>();
            _mockVisitationRepository = new Mock<IRepository<HomeVisitation, Guid>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _nurseService = new NurseService(
                _mockVisitationRepository.Object,
                _mockNurseRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object
            );
        }

        [Test]
        public async Task GetNurseProfileAsync_ReturnsProfile_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            var nurseProfile = new NurseProfile
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                UserId = Guid.Parse(userId)
            };

            _mockNurseRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<NurseProfile, bool>>>()))
                .ReturnsAsync(nurseProfile);

            // Act
            var result = await _nurseService.GetNurseProfileAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("John"));
            Assert.That(result.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task GetNurseProfileAsync_ReturnsNull_WhenProfileDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            _mockNurseRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<NurseProfile, bool>>>()))
                .ReturnsAsync((NurseProfile)null!);

            // Act
            var result = await _nurseService.GetNurseProfileAsync();

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}