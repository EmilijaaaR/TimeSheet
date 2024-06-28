using Domain.Entities;
using Domain.Enums;
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
    public class TimesheetRepositoryTests
    {
        private TimeSheetDbContext _dbContext;
        private ITimesheetRepository _timesheetRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TimeSheetDbContext(options);
            _timesheetRepository = new TimesheetRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
        [Test]
        public async Task InsertAsync_ShouldInsertTimesheet()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };
            var category2 = new Category { Id = 2, Name = "Test Category2" };
            var country = new Country { Id = 1, Name = "Test Country" };
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 1,
                Status = ProjectStatus.Active,
                Client = client,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var client2 = new Client
            {
                Id = 2,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 2,
                Status = ProjectStatus.Active,
                Client = client2,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var user = new User { Id = 1, FirstName = "Test User", LastName = "Test User", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" };
            var timesheet1 = new Timesheet
            {
                Id = 1,
                UserId = 1,
                User = user,
                Date = DateTime.UtcNow,
                ProjectId = 1,
                CategoryId = 1,
                HoursWorked = 8,
                Description = "Worked on project",
                OverTime = 2,
                Project = project,
                Category = category
            };
            var timesheet2 = new Timesheet
            {
                Id = 2,
                UserId = 2,
                User = new User { Id = 2, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" },
                Date = DateTime.UtcNow,
                ProjectId = 2,
                CategoryId = 1,
                HoursWorked = 6,
                Description = "Worked on another project",
                OverTime = 1,
                Project = project2,
                Category = category
            };

            var timesheets = new List<Timesheet> {timesheet1, timesheet2};
            // Act
            await _timesheetRepository.InsertAsync(timesheets);
            await _dbContext.SaveChangesAsync();

            //Assert
            Assert.AreEqual(2, _dbContext.Timesheets.Count());
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteTimesheet()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };
            var category2 = new Category { Id = 2, Name = "Test Category2" };
            var country = new Country { Id = 1, Name = "Test Country" };
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 1,
                Status = ProjectStatus.Active,
                Client = client,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var client2 = new Client
            {
                Id = 2,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 2,
                Status = ProjectStatus.Active,
                Client = client2,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var user = new User { Id = 1, FirstName = "Test User", LastName = "Test User", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" };
            var timesheet1 = new Timesheet
            {
                Id = 1,
                UserId = 1,
                User = user,
                Date = DateTime.UtcNow,
                ProjectId = 1,
                CategoryId = 1,
                HoursWorked = 8,
                Description = "Worked on project",
                OverTime = 2,
                Project = project,
                Category = category
            };
            var timesheet2 = new Timesheet
            {
                Id = 2,
                UserId = 2,
                User = new User { Id = 2, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" },
                Date = DateTime.UtcNow,
                ProjectId = 2,
                CategoryId = 1,
                HoursWorked = 6,
                Description = "Worked on another project",
                OverTime = 1,
                Project = project2,
                Category = category
            };

            var timesheets = new List<Timesheet> { timesheet1, timesheet2 };
            _dbContext.Timesheets.AddRange(timesheets);
            await _dbContext.SaveChangesAsync();

            // Act
            await _timesheetRepository.DeleteAsync(timesheet1);
            await _dbContext.SaveChangesAsync();

            //Assert
            Assert.AreEqual(1, _dbContext.Timesheets.Count());
        }

        [Test]
        public async Task GetTimesheetsAsync_ShouldReturnCorrectTimesheets()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };
            var category2 = new Category { Id = 2, Name = "Test Category2" };
            var country = new Country { Id = 1, Name = "Test Country" };
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 1,
                Status = ProjectStatus.Active,
                Client = client,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var client2 = new Client
            {
                Id = 2,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 2,
                Status = ProjectStatus.Active,
                Client = client2,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var user = new User { Id = 1, FirstName = "Test User", LastName = "Test User", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" };
            var timesheet1 = new Timesheet
            {
                Id = 1,
                UserId = 1,
                User = user,
                Date = new DateTime(2023, 6, 1),
                ProjectId = 1,
                CategoryId = 1,
                HoursWorked = 8,
                Description = "Worked on project",
                OverTime = 2,
                Project = project,
                Category = category
            };
            var timesheet2 = new Timesheet
            {
                Id = 2,
                UserId = 2,
                User = new User { Id = 2, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" },
                Date = DateTime.UtcNow,
                ProjectId = 2,
                CategoryId = 1,
                HoursWorked = 6,
                Description = "Worked on another project",
                OverTime = 1,
                Project = project2,
                Category = category
            };
            int userId = 1;
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2025, 1, 1);
            var timesheets = new List<Timesheet> { timesheet1, timesheet2 };
            _dbContext.Timesheets.AddRange(timesheets);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timesheetRepository.GetTimesheetsAsync(userId, startDate, endDate);
            await _dbContext.SaveChangesAsync();

            //Assert
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCorrectTimesheets()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };
            var category2 = new Category { Id = 2, Name = "Test Category2" };
            var country = new Country { Id = 1, Name = "Test Country" };
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 1,
                Status = ProjectStatus.Active,
                Client = client,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var client2 = new Client
            {
                Id = 2,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = country
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 2,
                Status = ProjectStatus.Active,
                Client = client2,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var user = new User { Id = 1, FirstName = "Test User", LastName = "Test User", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" };
            var timesheet1 = new Timesheet
            {
                Id = 1,
                UserId = 1,
                User = user,
                Date = new DateTime(2023, 6, 1),
                ProjectId = 1,
                CategoryId = 1,
                HoursWorked = 8,
                Description = "Worked on project",
                OverTime = 2,
                Project = project,
                Category = category
            };
            var timesheet2 = new Timesheet
            {
                Id = 2,
                UserId = 2,
                User = new User { Id = 2, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" },
                Date = DateTime.UtcNow,
                ProjectId = 2,
                CategoryId = 1,
                HoursWorked = 6,
                Description = "Worked on another project",
                OverTime = 1,
                Project = project2,
                Category = category
            };
            var timesheets = new List<Timesheet> { timesheet1, timesheet2 };
            _dbContext.Timesheets.AddRange(timesheets);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _timesheetRepository.GetByIdAsync(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }
    }
}
