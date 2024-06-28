using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.NUnitTests.Repositories
{
    [TestFixture]
    public class CountryRepositoryTests
    {
        private TimeSheetDbContext _dbContext;
        private ICountryRepository _countryRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TimeSheetDbContext(options);
            _countryRepository = new CountryRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldAddCountryToDatabase()
        {
            var country = new Country { Id = 1, Name = "Test Country" };

            await _countryRepository.InsertAsync(country);
            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Countries.FindAsync(country.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(country.Name, result.Name);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCountryWithClients()
        {
            var country = new Country { Id = 1, Name = "Test Country" };
            var client = new Client
            {
                Id = 1,
                Name = "Test Client",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country,
            };
            _dbContext.Countries.Add(country);
            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();

            var result = await _countryRepository.GetByIdAsync(country.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(country.Name, result.Name);
            Assert.AreEqual(1, result.Clients.Count);
            Assert.AreEqual(client.Name, result.Clients.First().Name);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveCountryFromDatabase()
        {
            var country = new Country { Id = 1, Name = "Test Country" };
            _dbContext.Countries.Add(country);
            await _dbContext.SaveChangesAsync();

            await _countryRepository.DeleteAsync(country);
            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Countries.FindAsync(country.Id);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCountriesWithClients()
        {
            var country1 = new Country { Id = 1, Name = "Test Country 1" };
            var country2 = new Country { Id = 2, Name = "Test Country 2" };
            var client = new Client 
            {
                Id = 1, 
                Name = "Test Client", 
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country1,
            };
            _dbContext.Countries.AddRange(country1, country2);
            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();

            var result = await _countryRepository.GetAllAsync();
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(1, resultList.First().Clients.Count);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueIfCountryExists()
        {
            var country = new Country { Id = 1, Name = "Test Country" };
            _dbContext.Countries.Add(country);
            await _dbContext.SaveChangesAsync();

            var result = await _countryRepository.ExistsAsync(country.Id);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseIfCountryDoesNotExist()
        {
            var result = await _countryRepository.ExistsAsync(1);
            Assert.IsFalse(result);
        }
    }
}
