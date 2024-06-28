using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.NUnitTests.Repositories
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        private TimeSheetDbContext _dbContext;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new TimeSheetDbContext(options);
            _unitOfWork = new UnitOfWork(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _unitOfWork.Dispose();
        }

        [Test]
        public void CategoryRepository_ShouldReturnInstanceOfCategoryRepository()
        {
            // Act
            var result = _unitOfWork.CategoryRepository;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CategoryRepository>(result);
        }

        [Test]
        public void ClientRepository_ShouldReturnInstanceOfClientRepository()
        {
            // Act
            var result = _unitOfWork.ClientRepository;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ClientRepository>(result);
        }

        [Test]
        public void CountryRepository_ShouldReturnInstanceOfCountryRepository()
        {
            // Act
            var result = _unitOfWork.CountryRepository;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CountryRepository>(result);
        }

        [Test]
        public void ProjectRepository_ShouldReturnInstanceOfProjectRepository()
        {
            // Act
            var result = _unitOfWork.ProjectRepository;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ProjectRepository>(result);
        }

        [Test]
        public void TimesheetRepository_ShouldReturnInstanceOfTimesheetRepository()
        {
            // Act
            var result = _unitOfWork.TimesheetRepository;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TimesheetRepository>(result);
        }

        [Test]
        public void UserRepository_ShouldReturnInstanceOfUserRepository()
        {
            // Act
            var result = _unitOfWork.UserRepository;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserRepository>(result);
        }

        [Test]
        public void ReportRepository_ShouldReturnInstanceOfReportRepository()
        {
            // Act
            var result = _unitOfWork.ReportRepository;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ReportRepository>(result);
        }

        [Test]
        public async Task SaveChangesAsync_ShouldSaveChanges()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" };
            _dbContext.Users.Add(user);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
