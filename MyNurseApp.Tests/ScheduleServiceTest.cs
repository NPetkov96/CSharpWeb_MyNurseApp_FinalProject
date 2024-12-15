using Microsoft.AspNetCore.Http;
using Moq;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.Manipulations;
using System.Linq.Expressions;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class ScheduleServiceTests
    {
        private readonly Mock<IRepository<HomeVisitation, Guid>> _mockVisitationRepository;
        private readonly Mock<IRepository<PatientProfile, Guid>> _mockPatientRepository;
        private readonly Mock<IRepository<NurseProfile, Guid>> _mockNurseRepository;
        private readonly Mock<IRepository<MedicalManipulation, Guid>> _mockManipulationRepository;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly ScheduleService _service;

        public ScheduleServiceTests()
        {
            _mockVisitationRepository = new Mock<IRepository<HomeVisitation, Guid>>();
            _mockPatientRepository = new Mock<IRepository<PatientProfile, Guid>>();
            _mockNurseRepository = new Mock<IRepository<NurseProfile, Guid>>();
            _mockManipulationRepository = new Mock<IRepository<MedicalManipulation, Guid>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _service = new ScheduleService(
                _mockManipulationRepository.Object,
                _mockVisitationRepository.Object,
                _mockPatientRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockNurseRepository.Object);
        }

        [Test]
        public void GetVisitationsForUserAsync_UserNotAuthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _service.GetVisitationsForUserAsync());
        }

        
        [Test]
        public void AddHomeVisitationAsync_InvalidDate_ThrowsInvalidOperationException()
        {
            // Arrange
            var model = new PatientAndHomeVisitationViewModel
            {
                HomeVisitation = new HomeVisitationViewModel
                {
                    DateTimeManipulation = DateTime.Now.AddDays(-1) // Invalid past date
                },
                MedicalManipulations = new List<MedicalManipulationsViewModel>()
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AddHomeVisitationAsync(model));
        }

        [Test]
        public void AssignVisitationToNurseAsync_InvalidNurse_ThrowsInvalidOperationException()
        {
            // Arrange
            var visitationId = Guid.NewGuid();

            _mockNurseRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<NurseProfile, bool>>>())).ReturnsAsync((NurseProfile)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AssignVisitationToNurseAsync(visitationId, Guid.NewGuid()));
        }

        [Test]
        public void DeleteHomeVisitationAsync_VisitationNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockVisitationRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((HomeVisitation)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.DeleteHomeVisitationAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteHomeVisitationAsync_ValidVisitation_DeletesSuccessfully()
        {
            // Arrange
            var visitation = new HomeVisitation { Id = Guid.NewGuid(), IsHomeVisitationConfirmed = false };

            _mockVisitationRepository.Setup(x => x.GetByIdAsync(visitation.Id)).ReturnsAsync(visitation);
            _mockVisitationRepository.Setup(x => x.DeleteAsync(visitation)).ReturnsAsync(true);

            // Act
            await _service.DeleteHomeVisitationAsync(visitation.Id);

            // Assert
            _mockVisitationRepository.Verify(x => x.DeleteAsync(visitation), Times.Once);
        }

    }

}

