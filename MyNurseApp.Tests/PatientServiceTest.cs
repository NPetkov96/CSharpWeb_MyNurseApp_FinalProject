using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.PatientProfile;
using System.Security.Claims;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class PatientServiceTests
    {
        private ApplicationDbContext _context = null!;
        private BaseRepository<PatientProfile, Guid> _patientRepository = null!;
        private PatientService _patientService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _patientRepository = new BaseRepository<PatientProfile, Guid>(_context);

            var userId = Guid.NewGuid();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }))
            });

            _patientService = new PatientService(_patientRepository, httpContextAccessor.Object);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddPatientAsync_AddsPatientSuccessfully()
        {
            // Arrange
            var patientViewModel = new PatientProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-30),
                UIN = "123456789",
                HomeAddress = "123 Test Street",
                PhoneNumber = "1234567890",
                EmergencyContactFullName = "Jane Doe",
                EmergencyContactPhone = "0987654321",
                Notes = "Test patient"
            };

            // Act
            await _patientService.AddPatientAsync(patientViewModel);

            // Assert
            var patient = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.UIN == "123456789");
            Assert.That(patient, Is.Not.Null);
            Assert.That(patient.FirstName, Is.EqualTo("John"));
        }


        [Test]
        public async Task GetPatientProfileAync_ReturnsCorrectPatient()
        {
            // Arrange
            var patient = new PatientProfile
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-30),
                UIN = "123456789",
                HomeAddress = "123 Test Street",
                PhoneNumber = "1234567890",
                EmergencyContactFullName = "Jane Doe",
                EmergencyContactPhone = "0987654321",
                UserId = Guid.NewGuid()
            };
            _context.PatientProfiles.Add(patient);
            await _context.SaveChangesAsync();

            // Act
            var result = await _patientService.GetPatientProfileAync(patient.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("John"));
            Assert.That(result.UIN, Is.EqualTo("123456789"));
        }

        [Test]
        public async Task DeletePatientAync_RemovesPatientSuccessfully()
        {
            // Arrange
            var patient = new PatientProfile
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                UIN = "123456789",
                UserId = Guid.NewGuid()
            };
            _context.PatientProfiles.Add(patient);
            await _context.SaveChangesAsync();

            // Act
            await _patientService.DeletePatientAync(patient.Id);

            // Assert
            var deletedPatient = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.Id == patient.Id);
            Assert.That(deletedPatient, Is.Null);
        }

        [Test]
        public void AddPatientAsync_ThrowsException_ForFutureDateOfBirth()
        {
            // Arrange
            var patientViewModel = new PatientProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddDays(1),
                UIN = "123456789",
                HomeAddress = "123 Test Street",
                PhoneNumber = "1234567890",
                EmergencyContactFullName = "Jane Doe",
                EmergencyContactPhone = "0987654321"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _patientService.AddPatientAsync(patientViewModel));
        }

        [Test]
        public void AddPatientAsync_ThrowsException_WhenPatientIsUnderage()
        {
            // Arrange
            var patientViewModel = new PatientProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-17), // Under 18
                UIN = "123456789",
                HomeAddress = "123 Test Street",
                PhoneNumber = "1234567890",
                EmergencyContactFullName = "Jane Doe",
                EmergencyContactPhone = "0987654321"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _patientService.AddPatientAsync(patientViewModel));
        }
    }

}