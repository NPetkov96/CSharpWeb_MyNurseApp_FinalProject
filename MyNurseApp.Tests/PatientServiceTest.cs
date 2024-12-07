using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class PatientServiceTest
    {
        private Mock<IRepository<PatientProfile, Guid>> _mockPatientRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private PatientService _patientService;

        [SetUp]
        public void Setup()
        {
            _mockPatientRepository = new Mock<IRepository<PatientProfile, Guid>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _patientService = new PatientService(
                _mockPatientRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object
            );
        }

        [Test]
        public async Task GetPatientProfileByUserIdAsync_ReturnsProfile_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            var patientProfile = new PatientProfile
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                UserId = Guid.Parse(userId)
            };

            _mockPatientRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<PatientProfile, bool>>>()))
                .ReturnsAsync(patientProfile);

            // Act
            var result = await _patientService.GetPatientProfileByUserIdAsync(_mockHttpContextAccessor.Object);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("John"));
            Assert.That(result.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task DeletePatientAync_RemovesPatientSuccessfully()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = new PatientProfile { Id = patientId, FirstName = "John" };

            _mockPatientRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(patient);

            _mockPatientRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<PatientProfile>()))
                .ReturnsAsync(true);

            // Act
            await _patientService.DeletePatientAync(patientId);

            // Assert
            _mockPatientRepository.Verify(repo => repo.DeleteAsync(It.Is<PatientProfile>(p => p.Id == patientId)), Times.Once);
        }

        [Test]
        public async Task GetAllPatientsAsync_ReturnsAllPatients()
        {
            // Arrange
            var patients = new List<PatientProfile>
        {
            new PatientProfile { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new PatientProfile { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

            _mockPatientRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(patients);

            // Act
            var result = await _patientService.GetAllPatientsAsync();

            // Assert
            Assert.That(result, Has.Exactly(2).Items);
            Assert.That(result.First().FirstName, Is.EqualTo("John"));
            Assert.That(result.Last().LastName, Is.EqualTo("Smith"));
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