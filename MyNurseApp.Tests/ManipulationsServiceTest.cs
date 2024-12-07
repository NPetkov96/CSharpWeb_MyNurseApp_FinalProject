using Moq;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data;
using System.Linq.Expressions;

namespace MyNurseApp.Tests
{
    [TestFixture]
    public class ManipulationsServiceTest
    {
        private Mock<IRepository<MedicalManipulation, Guid>>? _mockManipulationRepository;
        private ManipulationsService? _manipulationsService;

        [SetUp]
        public void Setup()
        {
            _mockManipulationRepository = new Mock<IRepository<MedicalManipulation, Guid>>();
            _manipulationsService = new ManipulationsService(_mockManipulationRepository.Object);
        }

        [Test]
        public async Task PatientBookManipulationAsync_AddsManipulationToList()
        {
            // Arrange
            var manipulationId = Guid.NewGuid();
            var manipulation = new MedicalManipulation
            {
                Id = manipulationId,
                Name = "Blood Test",
                Description = "A routine blood test",
                Price = 50.0m
            };

            _mockManipulationRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<MedicalManipulation, bool>>>()))
                .ReturnsAsync(manipulation);

            // Act
            var result = await _manipulationsService.PatientBookManipulationAsync(manipulationId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
            Assert.That(result.First().Name, Is.EqualTo("Blood Test"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCorrectManipulation()
        {
            // Arrange
            var manipulationId = Guid.NewGuid();
            var manipulation = new MedicalManipulation
            {
                Id = manipulationId,
                Name = "X-Ray",
                Description = "A chest X-ray",
                Price = 100.0m
            };

            _mockManipulationRepository
                .Setup(repo => repo.GetByIdAsync(manipulationId))
                .ReturnsAsync(manipulation);

            // Act
            var result = await _manipulationsService.GetByIdAsync(manipulationId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("X-Ray"));
        }

        [Test]
        public async Task GetAllManipulationsAsync_ReturnsOrderedManipulations()
        {
            // Arrange
            var manipulations = new List<MedicalManipulation>
        {
            new MedicalManipulation { Id = Guid.NewGuid(), Name = "Test A", Price = 30.0m },
            new MedicalManipulation { Id = Guid.NewGuid(), Name = "Test B", Price = 20.0m }
        };

            _mockManipulationRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(manipulations);

            // Act
            var result = await _manipulationsService.GetAllManipulationsAsync();

            // Assert
            Assert.That(result, Is.Ordered.By("Price"));
            Assert.That(result.First().Name, Is.EqualTo("Test B"));
        }

        [Test]
        public async Task RemoveManipulationAsync_DeletesManipulation_WhenItExists()
        {
            // Arrange
            var manipulationId = Guid.NewGuid();
            var manipulation = new MedicalManipulation
            {
                Id = manipulationId,
                Name = "CT Scan"
            };

            _mockManipulationRepository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<MedicalManipulation, bool>>>()))
                .ReturnsAsync(manipulation);

            // Act
            await _manipulationsService.RemoveManipulationAsync(manipulationId);

            // Assert
            _mockManipulationRepository.Verify(repo => repo.DeleteAsync(It.Is<MedicalManipulation>(m => m.Id == manipulationId)), Times.Once);
        }

        [Test]
        public void RemoveManipulationAsync_ThrowsException_WhenManipulationDoesNotExist()
        {
            // Arrange
            _mockManipulationRepository!
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<MedicalManipulation, bool>>>()))
                .ReturnsAsync((MedicalManipulation)null!);

            var manipulationId = Guid.NewGuid();

            // Act & Assert
            Assert.That(async () => await _manipulationsService!.RemoveManipulationAsync(manipulationId),
                Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("The manipulation doestn exist!"));
        }
    }
}