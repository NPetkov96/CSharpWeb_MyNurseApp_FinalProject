using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class AdminInformationServiceTest
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager = null!;
        private Mock<IRepository<NurseProfile, Guid>> _mockNurseRepository = null!;
        private Mock<IRepository<PatientProfile, Guid>> _mockPatientRepository = null!;
        private AdminInformationService _adminInformationService = null!;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

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

            var result = await _adminInformationService.DeleteUserAsync(userId);

            Assert.That(result, Is.True);
            _mockNurseRepository.Verify(repo => repo.DeleteAsync(It.Is<NurseProfile>(n => n.Id == user.Nurse.Id)), Times.Once);
            _mockUserManager.Verify(m => m.DeleteAsync(It.Is<ApplicationUser>(u => u.Id == userId)), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _mockUserManager.Setup(m => m.Users)
                .Returns(new List<ApplicationUser>().AsQueryable().BuildMockDbSet().Object);

            var result = await _adminInformationService.DeleteUserAsync(userId);

            Assert.That(result, Is.False);
        }

    }
}