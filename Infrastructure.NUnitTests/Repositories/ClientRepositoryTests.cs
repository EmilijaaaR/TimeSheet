using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Repositories;

namespace Infrastructure.NUnitTests.Repositories
{
    [TestFixture]
    public class ClientRepositoryTests
    {
        private TimeSheetDbContext _dbContext;
        private IClientRepository _clientRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TimeSheetDbContext(options);
            _clientRepository = new ClientRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldInsertClient()
        {
            // Arrange
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1
            };

            // Act
            await _clientRepository.InsertAsync(client);
            await _dbContext.SaveChangesAsync();

            //Assert
            Assert.AreEqual(1, _dbContext.Clients.Count());
            Assert.IsTrue(_dbContext.Clients.Contains(client));

            //var result = await _dbContext.Clients.FindAsync(client.Id);
            //Assert.IsNotNull(result);
            //Assert.AreEqual(client.Id, result.Id);
            //Assert.AreEqual(client.Name, result.Name);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCorrectClient()
        {
            var country1 = new Country { Id = 1, Name = "Country1" };
            var project1 = new Project { Id = 1, Name = "Project1", Description = "Desc", ClientId = 1 };
            // Arrange
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country1,
                Projects = new List<Project> { project1 }
            };
            await _dbContext.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _clientRepository.GetByIdAsync(client.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(client.Id, result.Id);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteClient()
        {
            var country1 = new Country { Id = 1, Name = "Country1" };
            var projects = new List<Project>
                {
                    new Project { Id = 1, Name = "Project1", Description = "Desc", ClientId = 1 },
                    new Project { Id = 2, Name = "Project2", Description = "Desc", ClientId = 1 }
                };
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country1,
                Projects = projects
            };
            await _dbContext.Countries.AddAsync(country1);
            await _dbContext.Projects.AddRangeAsync(projects);
            await _dbContext.Clients.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            // Act
            await _clientRepository.DeleteAsync(client);
            await _dbContext.SaveChangesAsync();
            // Assert
            var deletedClient = await _dbContext.Clients.FindAsync(client.Id);
            Assert.IsNull(deletedClient);
        }


        [Test]
        public async Task GetAllAsync_ShouldReturnAllClients()
        {
            // Arrange
            var country1 = new Country { Id = 1, Name = "Country1" };
            var country2 = new Country { Id = 2, Name = "Country2" };

            var project1 = new Project { Id = 1, Name = "Project1", Description = "Desc" };
            var project2 = new Project { Id = 2, Name = "Project2", Description = "Desc" };

            var clients = new List<Client>
            {
                new Client
                {
                    Id = 1,
                    Name = "Client1",
                    Address = "123 Main St",
                    City = "City",
                    PostalCode = "12345",
                    CountryId = 1,
                    Country = country1,
                    Projects = new List<Project> { project1 }
                },
                new Client
                {
                    Id = 2,
                    Name = "Client2",
                    Address = "456 Elm St",
                    City = "City2",
                    PostalCode = "54321",
                    CountryId = 2,
                    Country = country2,
                    Projects = new List<Project> { project2 }
                }
            };

            await _dbContext.Countries.AddRangeAsync(new[] { country1, country2 });
            await _dbContext.Projects.AddRangeAsync(new[] { project1, project2 });
            await _dbContext.Clients.AddRangeAsync(clients);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _clientRepository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(clients.Count, result.Count());

            foreach (var client in clients)
            {
                Assert.IsTrue(result.Any(c =>
                    c.Id == client.Id &&
                    c.Name == client.Name &&
                    c.Address == client.Address &&
                    c.City == client.City &&
                    c.PostalCode == client.PostalCode &&
                    c.CountryId == client.CountryId &&
                    c.Country.Name == client.Country.Name &&
                    c.Projects.Any(p => p.Id == client.Projects.First().Id)));
            }
        }


        [Test]
        public async Task GetClientsByUserIdAsync_ShouldReturnClients()
        {
            // Arrange
            int userId = 1;
            var clients = new List<Client>
            {
                new Client
                {
                    Id = 1,
                    Name = "Client1",
                    Address = "123 Main St",
                    City = "City2",
                    PostalCode = "12345",
                    CountryId = 2
                },
                new Client
                {
                    Id = 2,
                    Name = "Client2",
                    Address = "123 Main St",
                    City = "City2",
                    PostalCode = "12345",
                    CountryId = 2
                },
                new Client
                {
                    Id = 3,
                    Name = "OtherClient",
                    Address = "123 Main St",
                    City = "City2",
                    PostalCode = "12345",
                    CountryId = 2
                }
            };

            var clientUser = new ClientUser {UserId = userId, ClientId = 1 };

            await _dbContext.Clients.AddRangeAsync(clients);
            await _dbContext.ClientUsers.AddRangeAsync(clientUser);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _clientRepository.GetClientsByUserIdAsync(userId);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(c => c.ClientUsers.Any(cu => cu.UserId == userId)));
        }

        [Test]
        public async Task GetClientsByFiltersAsync_ShouldReturnFilteredClients()
        {
            // Arrange
            var clients = new List<Client>
        {
            new Client 
            { 
                Id = 1, 
                Name = "Client1",
                Address = "123 Main St",
                City = "City2",
                PostalCode = "12345",
                CountryId = 2
            },
            new Client 
            { 
                Id = 2, 
                Name = "Client2",
                Address = "123 Main St",
                City = "City2",
                PostalCode = "12345",
                CountryId = 2
            },
            new Client 
            { 
                Id = 3, 
                Name = "OtherClient",
                Address = "123 Main St",
                City = "City2",
                PostalCode = "12345",
                CountryId = 2
            }
        };
            await _dbContext.Clients.AddRangeAsync(clients);
            await _dbContext.SaveChangesAsync();

            int pageNumber = 1;
            int pageSize = 2;
            char letter = 'C';
            string searchTerm = "Client";

            // Act
            var result = await _clientRepository.GetClientsByFiltersAsync(pageNumber, pageSize, letter, searchTerm);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueIfClientExists()
        {
            // Arrange
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1
            };
            await _dbContext.Clients.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _clientRepository.ExistsAsync(client.Id);

            // Assert
            Assert.IsTrue(result);
        }
    }

}
