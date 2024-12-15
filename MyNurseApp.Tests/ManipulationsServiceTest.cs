using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository;
using MyNurseApp.Services.Data;
using MyNurseApp.Web.ViewModels.Manipulations;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class ManipulationsServiceTests
    {
        private ApplicationDbContext _context = null!;
        private BaseRepository<MedicalManipulation, Guid> _repository = null!;
        private ManipulationsService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new BaseRepository<MedicalManipulation, Guid>(_context);
            _service = new ManipulationsService(_repository);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddManipulationAsync_AddsSuccessfully()
        {
            var model = new MedicalManipulationsViewModel
            {
                Name = "Blood Test",
                Duration = 30,
                Description = "Routine blood test",
                Price = 20.00M
            };

            await _service.AddManipulationAsync(model);

            var manipulation = await _context.MedicalManipulations.FirstOrDefaultAsync(m => m.Name == "Blood Test");
            Assert.That(manipulation, Is.Not.Null);
            Assert.That(manipulation!.Price, Is.EqualTo(20.00M));
        }

        [Test]
        public void AddManipulationAsync_ThrowsException_WhenManipulationExists()
        {
            var manipulation = new MedicalManipulation
            {
                Id = Guid.NewGuid(),
                Name = "Blood Test",
                Duration = 30,
                Description = "Routine blood test",
                Price = 20.00M
            };
            _context.MedicalManipulations.Add(manipulation);
            _context.SaveChanges();

            var model = new MedicalManipulationsViewModel
            {
                Id = manipulation.Id,
                Name = "Blood Test",
                Duration = 30,
                Description = "Routine blood test",
                Price = 20.00M
            };

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AddManipulationAsync(model));
        }

        [Test]
        public async Task GetAllManipulationsAsync_ReturnsAllManipulations()
        {
            _context.MedicalManipulations.AddRange(
                new MedicalManipulation { Name = "Blood Test", Duration = 30, Price = 20.00M },
                new MedicalManipulation { Name = "X-Ray", Duration = 45, Price = 50.00M });
            await _context.SaveChangesAsync();

            var manipulations = await _service.GetAllManipulationsAsync(1,7);

            Assert.That(manipulations, Has.Exactly(2).Items);
            Assert.That(manipulations.First().Name, Is.EqualTo("Blood Test"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCorrectManipulation()
        {
            var manipulation = new MedicalManipulation
            {
                Id = Guid.NewGuid(),
                Name = "MRI Scan",
                Duration = 60,
                Price = 200.00M
            };
            _context.MedicalManipulations.Add(manipulation);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(manipulation.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("MRI Scan"));
        }

        [Test]
        public void GetByIdAsync_ThrowsException_WhenManipulationNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            Assert.ThrowsAsync<NullReferenceException>(async () =>
                await _service.GetByIdAsync(nonExistentId), "Expected NullReferenceException when manipulation is not found.");
        }

        [Test]
        public async Task RemoveManipulationAsync_RemovesSuccessfully()
        {
            var manipulation = new MedicalManipulation
            {
                Id = Guid.NewGuid(),
                Name = "Blood Test",
                Duration = 30,
                Price = 20.00M
            };
            _context.MedicalManipulations.Add(manipulation);
            await _context.SaveChangesAsync();

            await _service.RemoveManipulationAsync(manipulation.Id);

            var result = await _context.MedicalManipulations.FirstOrDefaultAsync(m => m.Id == manipulation.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void RemoveManipulationAsync_ThrowsException_WhenManipulationNotFound()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.RemoveManipulationAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task EditManipulationAsync_UpdatesSuccessfully()
        {
            var manipulation = new MedicalManipulation
            {
                Id = Guid.NewGuid(),
                Name = "Initial Manipulation",
                Duration = 30,
                Description = "Initial description",
                Price = 100.00m
            };

            _context.MedicalManipulations.Add(manipulation);
            await _context.SaveChangesAsync();

            var updatedManipulation = new MedicalManipulationsViewModel
            {
                Id = manipulation.Id,
                Name = "Updated Manipulation",
                Duration = 45,
                Description = "Updated description",
                Price = 150.00m
            };

            _context.Entry(manipulation).State = EntityState.Detached;

            await _service.EditManipulationAsync(updatedManipulation);

            var result = await _context.MedicalManipulations.FirstOrDefaultAsync(m => m.Id == manipulation.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Manipulation"));
            Assert.That(result.Description, Is.EqualTo("Updated description"));
            Assert.That(result.Price, Is.EqualTo(150.00m));
        }

        [Test]
        public async Task SearchManipulationsAsync_ReturnsFilteredResults()
        {
            _context.MedicalManipulations.AddRange(
                new MedicalManipulation { Name = "Blood Test", Duration = 30, Price = 20.00M },
                new MedicalManipulation { Name = "X-Ray", Duration = 45, Price = 50.00M });
            await _context.SaveChangesAsync();

            var results = await _service.SearchManipulationsAsync("Blood");

            Assert.That(results, Has.Exactly(1).Items);
            Assert.That(results.First().Name, Is.EqualTo("Blood Test"));
        }

        [Test]
        public async Task SearchManipulationsAsync_ReturnsAll_WhenQueryIsEmpty()
        {
            _context.MedicalManipulations.AddRange(
                new MedicalManipulation { Name = "Blood Test", Duration = 30, Price = 20.00M },
                new MedicalManipulation { Name = "X-Ray", Duration = 45, Price = 50.00M });
            await _context.SaveChangesAsync();

            var results = await _service.SearchManipulationsAsync("");

            Assert.That(results, Has.Exactly(2).Items);
        }
    }
}