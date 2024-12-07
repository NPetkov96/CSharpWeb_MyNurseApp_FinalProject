using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Data;
using MyNurseApp.Services.Data;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class AdminInformationServiceTest
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IRepository<NurseProfile, Guid>> _mockNurseRepository;
        private Mock<IRepository<PatientProfile, Guid>> _mockPatientRepository;
        private AdminInformationService _adminInformationService;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _mockNurseRepository = new Mock<IRepository<NurseProfile, Guid>>();
            _mockPatientRepository = new Mock<IRepository<PatientProfile, Guid>>();

            _adminInformationService = new AdminInformationService(
                _mockUserManager.Object,
                _mockNurseRepository.Object,
                _mockPatientRepository.Object
            );
        }


        [Test]
        public async Task DeleteUserAsync_DeletesUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                Nurse = new NurseProfile { Id = Guid.NewGuid() },
                Patient = null
            };

            _mockUserManager.Setup(m => m.Users)
                .Returns(new List<ApplicationUser> { user }.AsQueryable().BuildMockDbSet().Object);

            _mockUserManager.Setup(m => m.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            _mockNurseRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<NurseProfile>()))
                .ReturnsAsync(true);

            // Act
            var result = await _adminInformationService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result, Is.True);
            _mockNurseRepository.Verify(repo => repo.DeleteAsync(It.Is<NurseProfile>(n => n.Id == user.Nurse.Id)), Times.Once);
            _mockUserManager.Verify(m => m.DeleteAsync(It.Is<ApplicationUser>(u => u.Id == userId)), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserManager.Setup(m => m.Users)
                .Returns(new List<ApplicationUser>().AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _adminInformationService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result, Is.False);
        }

        private Mock<DbSet<T>> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }
    }
}