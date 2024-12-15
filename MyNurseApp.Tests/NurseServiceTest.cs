using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Data;
using MyNurseApp.Services.Data;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Common.Enums;
using MyNurseApp.Data.Repository;
using MyNurseApp.Web.ViewModels.NurseProfile;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class NurseServiceTests
    {
        private ApplicationDbContext _context = null!;
        private BaseRepository<NurseProfile, Guid> _nurseRepository = null!;
        private BaseRepository<HomeVisitation, Guid> _visitationRepository = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
        private NurseService _nurseService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _nurseRepository = new BaseRepository<NurseProfile, Guid>(_context);
            _visitationRepository = new BaseRepository<HomeVisitation, Guid>(_context);

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }))
            });

            var userManagerMock = MockUserManager();
            _nurseService = new NurseService(_visitationRepository, _nurseRepository, _httpContextAccessorMock.Object, userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task RegisterNurseAsync_AddsNurseSuccessfully()
        {
            // Arrange
            var nurseViewModel = new NurseProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                MedicalLicenseNumber = "ML12345",
                PhoneNumber = "1234567890",
                Education = NurseEducation.AssociateDegree,
                YearsOfExperience = 5
            };

            // Act
            await _nurseService.RegisterNurseAsync(nurseViewModel);

            // Assert
            var nurse = await _context.NurseProfiles.FirstOrDefaultAsync(n => n.MedicalLicenseNumber == "ML12345");
            Assert.That(nurse, Is.Not.Null);
            Assert.That(nurse.FirstName, Is.EqualTo("John"));
        }

        [Test]
        public async Task DeleteNurseProfileAync_RemovesNurseSuccessfully()
        {
            // Arrange
            var nurse = new NurseProfile
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                MedicalLicenseNumber = "ML54321",
                PhoneNumber = "0987654321",
                Education = NurseEducation.AssociateDegree,
                YearsOfExperience = 10,
                IsRegistrated = true
            };

            _context.NurseProfiles.Add(nurse);
            await _context.SaveChangesAsync();

            // Act
            await _nurseService.DeleteNurseProfileAync(nurse.Id);

            // Assert
            var deletedNurse = await _context.NurseProfiles.FirstOrDefaultAsync(n => n.Id == nurse.Id);
            Assert.That(deletedNurse, Is.Null);
        }

        [Test]
        public async Task GetNurseProfileAsync_ReturnsCorrectNurse()
        {
            // Arrange
            var nurseId = Guid.NewGuid();
            var nurse = new NurseProfile
            {
                Id = nurseId,
                FirstName = "Alice",
                LastName = "Smith",
                MedicalLicenseNumber = "ML67890",
                PhoneNumber = "1234509876",
                Education = NurseEducation.AssociateDegree,
                YearsOfExperience = 7,
                UserId = Guid.Parse(_httpContextAccessorMock.Object.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!)
            };

            _context.NurseProfiles.Add(nurse);
            await _context.SaveChangesAsync();

            // Act
            var result = await _nurseService.GetNurseProfileAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("Alice"));
            Assert.That(result.MedicalLicenseNumber, Is.EqualTo("ML67890"));
        }

        [Test]
        public async Task RegisterNurseAsync_ThrowsException_WhenLicenseExists()
        {
            // Arrange
            var nurse = new NurseProfile
            {
                Id = Guid.NewGuid(),
                FirstName = "Charlie",
                LastName = "Brown",
                PhoneNumber = "1234567890",
                MedicalLicenseNumber = "ML99999",
                IsRegistrated = true
            };

            _context.NurseProfiles.Add(nurse);
            await _context.SaveChangesAsync();

            var nurseViewModel = new NurseProfileViewModel
            {
                FirstName = "David",
                LastName = "Green",
                MedicalLicenseNumber = "ML99999",
                PhoneNumber = "4567891230",
                Education = NurseEducation.AssociateDegree,
                YearsOfExperience = 5
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _nurseService.RegisterNurseAsync(nurseViewModel));
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
    }

}