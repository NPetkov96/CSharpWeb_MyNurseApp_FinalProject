using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNurseApp.Common.Enums;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels;
using MyNurseApp.Web.ViewModels.HomeVisitation;
using MyNurseApp.Web.ViewModels.Manipulations;
using MyNurseApp.Web.ViewModels.PatientProfile;
using System.Security.Claims;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class ScheduleServiceTests
    {
        private ApplicationDbContext _context = null!;
        private BaseRepository<HomeVisitation, Guid> _visitationRepository = null!;
        private BaseRepository<PatientProfile, Guid> _patientRepository = null!;
        private BaseRepository<NurseProfile, Guid> _nurseRepository = null!;
        private BaseRepository<MedicalManipulation, Guid> _manipulationRepository = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
        private ScheduleService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _visitationRepository = new BaseRepository<HomeVisitation, Guid>(_context);
            _patientRepository = new BaseRepository<PatientProfile, Guid>(_context);
            _nurseRepository = new BaseRepository<NurseProfile, Guid>(_context);
            _manipulationRepository = new BaseRepository<MedicalManipulation, Guid>(_context);

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _service = new ScheduleService(
                _manipulationRepository,
                _visitationRepository,
                _patientRepository,
                _httpContextAccessorMock.Object,
                _nurseRepository
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetVisitationsForUserAsync_ThrowsUnauthorizedAccessException_WhenUserNotAuthenticated()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.GetVisitationsForUserAsync());
        }

        [Test]
        public async Task GetVisitationsForUserAsync_ReturnsEmpty_WhenNoVisitationsExist()
        {
            var userId = Guid.NewGuid().ToString();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }))
            });

            _context.PatientProfiles.Add(new PatientProfile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                FirstName = "John",
                LastName = "Doe",
                HomeAddress = "123 Test Street", 
                PhoneNumber = "1234567890", 
                UIN = "123456789", 
                EmergencyContactFullName = "Jane Doe", 
                EmergencyContactPhone = "0987654321"
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetVisitationsForUserAsync();

            Assert.That(result, Is.Empty, "Expected no visitations for the user.");
        }


        [Test]
        public async Task AddHomeVisitationAsync_AddsSuccessfully()
        {
            var patientId = Guid.NewGuid();
            _context.PatientProfiles.Add(new PatientProfile
            {
                Id = patientId,
                FirstName = "John",
                LastName = "Doe",
                HomeAddress = "123 Test Street",
                PhoneNumber = "1234567890",
                UIN = "123456789",
                EmergencyContactFullName = "Jane Doe",
                EmergencyContactPhone = "0987654321"
            });
            await _context.SaveChangesAsync();

            var model = new PatientAndHomeVisitationViewModel
            {
                PatientProfile = new PatientProfileViewModel
                {
                    Id = patientId
                },
                HomeVisitation = new HomeVisitationViewModel
                {
                    DateTimeManipulation = DateTime.Now.AddDays(1),
                    PaymentMethod = PaymentMethod.Cash,
                    PriceForVisitation = 100m
                },
                MedicalManipulations = new List<MedicalManipulationsViewModel>()
            };

            await _service.AddHomeVisitationAsync(model);

            var visitation = await _context.HomeVisitations.FirstOrDefaultAsync();
            Assert.That(visitation, Is.Not.Null);
            Assert.That(visitation!.PriceForVisitation, Is.EqualTo(100m));
        }


        [Test]
        public async Task DeleteHomeVisitationAsync_ThrowsException_WhenVisitationNotFound()
        {
            var visitationId = Guid.NewGuid();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.DeleteHomeVisitationAsync(visitationId));
        }

        [Test]
        public async Task DeleteHomeVisitationAsync_RemovesVisitationSuccessfully()
        {
            var visitation = new HomeVisitation
            {
                Id = Guid.NewGuid(),
                DateTimeManipulation = DateTime.Now.AddDays(1),
                PriceForVisitation = 100m,
                PatientId = Guid.NewGuid()
            };

            _context.HomeVisitations.Add(visitation);
            await _context.SaveChangesAsync();

            await _service.DeleteHomeVisitationAsync(visitation.Id);

            var result = await _context.HomeVisitations.FirstOrDefaultAsync(v => v.Id == visitation.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AssignVisitationToNurseAsync_AssignsSuccessfully()
        {
            var nurse = new NurseProfile
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Smith",
                MedicalLicenseNumber = "ML12345", 
                PhoneNumber = "1234567890"
            };
            var visitation = new HomeVisitation
            {
                Id = Guid.NewGuid(),
                DateTimeManipulation = DateTime.Now.AddDays(1),
                PriceForVisitation = 100m
            };

            _context.NurseProfiles.Add(nurse);
            _context.HomeVisitations.Add(visitation);
            await _context.SaveChangesAsync();

            await _service.AssignVisitationToNurseAsync(visitation.Id, nurse.Id);

            var updatedNurse = await _context.NurseProfiles
                .Include(n => n.HomeVisitations)
                .FirstOrDefaultAsync(n => n.Id == nurse.Id);

            Assert.That(updatedNurse, Is.Not.Null);
            Assert.That(updatedNurse!.HomeVisitations, Has.Exactly(1).Items);
            Assert.That(updatedNurse.HomeVisitations.First().Id, Is.EqualTo(visitation.Id));
        }
    }
}

