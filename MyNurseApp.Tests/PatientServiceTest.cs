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
    public class PatientServiceTest
    {
        private Mock<IRepository<PatientProfile, Guid>> _mockPatientRepository = null!;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor = null!;
        private PatientService _patientService = null!;

        [SetUp]
        public void Setup()
        {
            _mockPatientRepository = new Mock<IRepository<PatientProfile, Guid>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _patientService = new PatientService(
                _mockPatientRepository.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Test]
        public async Task GetPatientProfileByUserIdAsync_ReturnsProfile_WhenUserExists()
        {
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

            var result = await _patientService.GetPatientProfileByUserIdAsync(_mockHttpContextAccessor.Object);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("John"));
            Assert.That(result.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task DeletePatientAync_RemovesPatientSuccessfully()
        {
            var patientId = Guid.NewGuid();
            var patient = new PatientProfile { Id = patientId, FirstName = "John" };

            _mockPatientRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(patient);

            _mockPatientRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<PatientProfile>()))
                .ReturnsAsync(true);

            await _patientService.DeletePatientAync(patientId);

            _mockPatientRepository.Verify(repo => repo.DeleteAsync(It.Is<PatientProfile>(p => p.Id == patientId)), Times.Once);
        }

        [Test]
        public async Task GetAllPatientsAsync_ReturnsAllPatients()
        {
            var patients = new List<PatientProfile>
        {
            new PatientProfile { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new PatientProfile { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

            _mockPatientRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(patients);

            var result = await _patientService.GetAllPatientsAsync();

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