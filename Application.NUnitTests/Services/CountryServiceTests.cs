using Moq;
using Domain.Entities;
using Domain.Repositories;
using Application.Services;
using NUnit.Framework.Legacy;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class CountryServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICountryRepository> _mockCountryRepository;
        private CountryService _countryService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCountryRepository = new Mock<ICountryRepository>();

            _mockUnitOfWork.Setup(u => u.CountryRepository).Returns(_mockCountryRepository.Object);

            _countryService = new CountryService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task CreateAsync_ValidCountry_ReturnsCountryView()
        {
            // Arrange
            var countryName = "TestCountry";

            Country createdCountry = null;
            _mockCountryRepository.Setup(repo => repo.InsertAsync(It.IsAny<Country>()))
                .Callback<Country>(c => createdCountry = c);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _countryService.CreateAsync(countryName);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(countryName, result.Name);

            _mockCountryRepository.Verify(repo => repo.InsertAsync(It.IsAny<Country>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ExistingCountry_ReturnsCountryView()
        {
            // Arrange
            var countryId = 1;
            var existingCountry = new Country { Id = countryId, Name = "ExistingCountry" };

            _mockCountryRepository.Setup(repo => repo.GetByIdAsync(countryId)).ReturnsAsync(existingCountry);

            // Act
            var result = await _countryService.GetByIdAsync(countryId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(existingCountry.Name, result.Name);

            _mockCountryRepository.Verify(repo => repo.GetByIdAsync(countryId), Times.Once);
        }

        [Test]
        public void GetByIdAsync_NonExistingCountry_ThrowsArgumentException()
        {
            // Arrange
            var countryId = 1;

            _mockCountryRepository.Setup(repo => repo.GetByIdAsync(countryId)).ReturnsAsync((Country)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _countryService.GetByIdAsync(countryId));
            ClassicAssert.AreEqual("Country does not exist.", ex.Message);

            _mockCountryRepository.Verify(repo => repo.GetByIdAsync(countryId), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ExistingCountry_ReturnsUpdatedCountryView()
        {
            // Arrange
            var countryId = 1;
            var updatedName = "UpdatedCountry";
            var existingCountry = new Country { Id = countryId, Name = "ExistingCountry" };

            _mockCountryRepository.Setup(repo => repo.GetByIdAsync(countryId)).ReturnsAsync(existingCountry);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _countryService.UpdateAsync(countryId, updatedName);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(updatedName, result.Name);

            _mockCountryRepository.Verify(repo => repo.GetByIdAsync(countryId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateAsync_NonExistingCountry_ThrowsArgumentException()
        {
            // Arrange
            var countryId = 1;
            var updatedName = "UpdatedCountry";

            _mockCountryRepository.Setup(repo => repo.GetByIdAsync(countryId)).ReturnsAsync((Country)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _countryService.UpdateAsync(countryId, updatedName));
            ClassicAssert.AreEqual("Country not found", ex.Message);

            _mockCountryRepository.Verify(repo => repo.GetByIdAsync(countryId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ExistingCountry_DeletesCountry()
        {
            // Arrange
            var countryId = 1;
            var existingCountry = new Country { Id = countryId, Name = "ExistingCountry" };

            _mockCountryRepository.Setup(repo => repo.GetByIdAsync(countryId)).ReturnsAsync(existingCountry);
            _mockCountryRepository.Setup(repo => repo.DeleteAsync(existingCountry)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            await _countryService.DeleteAsync(countryId);

            // Assert
            _mockCountryRepository.Verify(repo => repo.GetByIdAsync(countryId), Times.Once);
            _mockCountryRepository.Verify(repo => repo.DeleteAsync(existingCountry), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteAsync_NonExistingCountry_ThrowsArgumentException()
        {
            // Arrange
            var countryId = 1;

            _mockCountryRepository.Setup(repo => repo.GetByIdAsync(countryId)).ReturnsAsync((Country)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _countryService.DeleteAsync(countryId));
            ClassicAssert.AreEqual("Country not found", ex.Message);

            _mockCountryRepository.Verify(repo => repo.GetByIdAsync(countryId), Times.Once);
            _mockCountryRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Country>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_ReturnsCountryViews()
        {
            // Arrange
            var countries = new List<Country>
            {
                new Country { Id = 1, Name = "Country1" },
                new Country { Id = 2, Name = "Country2" }
            };

            _mockCountryRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(countries);

            // Act
            var result = await _countryService.GetAllAsync();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
            ClassicAssert.AreEqual("Country1", result.First().Name);
            ClassicAssert.AreEqual("Country2", result.Last().Name);

            _mockCountryRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}
