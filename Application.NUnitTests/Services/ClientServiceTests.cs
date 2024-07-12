using Moq;
using Domain.Entities;
using Domain.Repositories;
using Application.Services;
using API.RequestEntities;
using NUnit.Framework.Legacy;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class ClientServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<ICountryRepository> _mockCountryRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private ClientService _clientService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockClientRepository = new Mock<IClientRepository>();
            _mockCountryRepository = new Mock<ICountryRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _mockUnitOfWork.Setup(u => u.ClientRepository).Returns(_mockClientRepository.Object);
            _mockUnitOfWork.Setup(u => u.CountryRepository).Returns(_mockCountryRepository.Object);
            _mockUnitOfWork.Setup(u => u.UserRepository).Returns(_mockUserRepository.Object);

            _clientService = new ClientService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task CreateAsync_ValidClient_ReturnsClientView()
        {
            // Arrange
            var request = new ClientRequest
            {
                Name = "TestClient",
                Address = "TestAddress",
                City = "TestCity",
                PostalCode = "12345",
                CountryId = 1
            };

            _mockCountryRepository.Setup(repo => repo.ExistsAsync(request.CountryId)).ReturnsAsync(true);

            Client createdClient = null;
            _mockClientRepository.Setup(repo => repo.InsertAsync(It.IsAny<Client>()))
                .Callback<Client>(c => createdClient = c);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _clientService.CreateAsync(request);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(request.Name, result.Name);
            ClassicAssert.AreEqual(request.Address, result.Address);
            ClassicAssert.AreEqual(request.City, result.City);
            ClassicAssert.AreEqual(request.PostalCode, result.PostalCode);
            ClassicAssert.AreEqual(request.CountryId, result.CountryId);

            _mockCountryRepository.Verify(repo => repo.ExistsAsync(request.CountryId), Times.Once);
            _mockClientRepository.Verify(repo => repo.InsertAsync(It.IsAny<Client>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void CreateAsync_InvalidCountry_ThrowsArgumentException()
        {
            // Arrange
            var request = new ClientRequest
            {
                Name = "TestClient",
                Address = "TestAddress",
                City = "TestCity",
                PostalCode = "12345",
                CountryId = 1
            };

            _mockCountryRepository.Setup(repo => repo.ExistsAsync(request.CountryId)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _clientService.CreateAsync(request));
            ClassicAssert.AreEqual("Country does not exist.", ex.Message);

            _mockCountryRepository.Verify(repo => repo.ExistsAsync(request.CountryId), Times.Once);
            _mockClientRepository.Verify(repo => repo.InsertAsync(It.IsAny<Client>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_ExistingClient_ReturnsClientView()
        {
            // Arrange
            var clientId = 1;
            var existingClient = new Client { Id = clientId, Name = "ExistingClient" };

            _mockClientRepository.Setup(repo => repo.GetByIdAsync(clientId)).ReturnsAsync(existingClient);

            // Act
            var result = await _clientService.GetByIdAsync(clientId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(existingClient.Name, result.Name);

            _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
        }

        [Test]
        public void GetByIdAsync_NonExistingClient_ThrowsArgumentException()
        {
            // Arrange
            var clientId = 1;

            _mockClientRepository.Setup(repo => repo.GetByIdAsync(clientId)).ReturnsAsync((Client)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _clientService.GetByIdAsync(clientId));
            ClassicAssert.AreEqual("Client doesn't exist.", ex.Message);

            _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ExistingClient_ReturnsUpdatedClientView()
        {
            // Arrange
            var clientId = 1;
            var request = new ClientRequest
            {
                Name = "UpdatedClient",
                Address = "UpdatedAddress",
                City = "UpdatedCity",
                PostalCode = "54321",
                CountryId = 2
            };

            var existingClient = new Client { Id = clientId, Name = "ExistingClient" };

            _mockClientRepository.Setup(repo => repo.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
            _mockCountryRepository.Setup(repo => repo.ExistsAsync(request.CountryId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _clientService.UpdateAsync(clientId, request);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(request.Name, result.Name);
            ClassicAssert.AreEqual(request.Address, result.Address);
            ClassicAssert.AreEqual(request.City, result.City);
            ClassicAssert.AreEqual(request.PostalCode, result.PostalCode);
            ClassicAssert.AreEqual(request.CountryId, result.CountryId);

            _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
            _mockCountryRepository.Verify(repo => repo.ExistsAsync(request.CountryId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateAsync_NonExistingClient_ThrowsArgumentException()
        {
            // Arrange
            var clientId = 1;
            var request = new ClientRequest
            {
                Name = "UpdatedClient",
                Address = "UpdatedAddress",
                City = "UpdatedCity",
                PostalCode = "54321",
                CountryId = 2
            };

            _mockClientRepository.Setup(repo => repo.GetByIdAsync(clientId)).ReturnsAsync((Client)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _clientService.UpdateAsync(clientId, request));
            ClassicAssert.AreEqual("Client not found", ex.Message);

            _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
            _mockCountryRepository.Verify(repo => repo.ExistsAsync(request.CountryId), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public void UpdateAsync_InvalidCountry_ThrowsArgumentException()
        {
            // Arrange
            var clientId = 1;
            var request = new ClientRequest
            {
                Name = "UpdatedClient",
                Address = "UpdatedAddress",
                City = "UpdatedCity",
                PostalCode = "54321",
                CountryId = 2
            };

            var existingClient = new Client { Id = clientId, Name = "ExistingClient" };

            _mockClientRepository.Setup(repo => repo.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
            _mockCountryRepository.Setup(repo => repo.ExistsAsync(request.CountryId)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _clientService.UpdateAsync(clientId, request));
            ClassicAssert.AreEqual("Country does not exist.", ex.Message);

            _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
            _mockCountryRepository.Verify(repo => repo.ExistsAsync(request.CountryId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ExistingClient_DeletesClient()
        {
            // Arrange
            var clientId = 1;
            var existingClient = new Client { Id = clientId, Name = "ExistingClient" };

            _mockClientRepository.Setup(repo => repo.GetByIdAsync(clientId)).ReturnsAsync(existingClient);
            _mockClientRepository.Setup(repo => repo.DeleteAsync(existingClient)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            await _clientService.DeleteAsync(clientId);

            // Assert
            _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
            _mockClientRepository.Verify(repo => repo.DeleteAsync(existingClient), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteAsync_NonExistingClient_ThrowsArgumentException()
        {
            // Arrange
            var clientId = 1;

            _mockClientRepository.Setup(repo => repo.GetByIdAsync(clientId)).ReturnsAsync((Client)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _clientService.DeleteAsync(clientId));
            ClassicAssert.AreEqual("Client not found", ex.Message);

            _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId), Times.Once);
            _mockClientRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Client>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_ReturnsClientViews()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, Name = "Client1" },
                new Client { Id = 2, Name = "Client2" }
            };

            _mockClientRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(clients);

            // Act
            var result = await _clientService.GetAllAsync();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
            ClassicAssert.AreEqual("Client1", result.First().Name);
            ClassicAssert.AreEqual("Client2", result.Last().Name);

            _mockClientRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetClientsByFiltersAsync_ReturnsFilteredClientViews()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, Name = "ClientA" },
                new Client { Id = 2, Name = "ClientB" }
            };

            _mockClientRepository.Setup(repo => repo.GetClientsByFiltersAsync(1, 10, 'C', "Client"))
                .ReturnsAsync(clients);

            // Act
            var result = await _clientService.GetClientsByFiltersAsync(1, 10, 'C', "Client");

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
            ClassicAssert.AreEqual("ClientA", result.First().Name);
            ClassicAssert.AreEqual("ClientB", result.Last().Name);

            _mockClientRepository.Verify(repo => repo.GetClientsByFiltersAsync(1, 10, 'C', "Client"), Times.Once);
        }

        [Test]
        public async Task GetClientsByUserIdAsync_ValidUser_ReturnsClientViews()
        {
            // Arrange
            var userId = 1;
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1; 
            var clients = new List<Client>
            {
                new Client { Id = 1, Name = "Client1" },
                new Client { Id = 2, Name = "Client2" }
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockClientRepository.Setup(repo => repo.GetClientsByUserIdAsync(userId)).ReturnsAsync(clients);

            // Act
            var result = await _clientService.GetClientsByUserIdAsync(userId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
            ClassicAssert.AreEqual("Client1", result.First().Name);
            ClassicAssert.AreEqual("Client2", result.Last().Name);

            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _mockClientRepository.Verify(repo => repo.GetClientsByUserIdAsync(userId), Times.Once);
        }

        [Test]
        public void GetClientsByUserIdAsync_InvalidUser_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _clientService.GetClientsByUserIdAsync(userId));
            ClassicAssert.AreEqual("User does not exist.", ex.Message);

            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _mockClientRepository.Verify(repo => repo.GetClientsByUserIdAsync(userId), Times.Never);
        }
    }
}
