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
    public class ReportRepositoryTests
    {
        private TimeSheetDbContext _dbContext;
        private IReportRepository _reportRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TimeSheetDbContext(options);
            _reportRepository = new ReportRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetReportDataAsync_ShouldReturnTimesheetsFilteredByUserId()
        {
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
                Project = new Project 
                { 
                    Id = 1, 
                    Name = "Test Project",
                    Description = "Test Description",
                    ClientId = 1,
                    Status = ProjectStatus.Active,
                    Client = new Client
                    {
                        Id = 1,
                        Name = "Client1",
                        Address = "123 Main St",
                        City = "City",
                        PostalCode = "12345",
                        CountryId = 1,
                        Country = new Country{ Id = 1, Name = "Test Country" }
                    },
                    Users = new List<User>(),
                    ProjectUsers = new List<ProjectUser>(),
                    Timesheets = new List<Timesheet>()
                },
                Category = new Category { Id = 1, Name = "Test Category" }
            };
            var timesheet2 = new Timesheet
            {
                Id = 2,
                UserId = 2,
                User = new User { Id = 2, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" },
                Date = DateTime.UtcNow,
                ProjectId = 2,
                CategoryId = 2,
                HoursWorked = 6,
                Description = "Worked on another project",
                OverTime = 1,
                Project = new Project 
                { 
                    Id = 2, 
                    Name = "Another Project",
                    Description = "Test Description",
                    ClientId = 1,
                    Status = ProjectStatus.Active,
                    Users = new List<User>(),
                    ProjectUsers = new List<ProjectUser>(),
                    Timesheets = new List<Timesheet>()
                },
                Category = new Category { Id = 2, Name = "Another Category" }
            };
            _dbContext.Users.Add(user);
            _dbContext.Timesheets.AddRange(timesheet1, timesheet2);
            await _dbContext.SaveChangesAsync();

            var result = await _reportRepository.GetReportDataAsync(1, null, null, null, null, null);

            Assert.AreEqual(1, result.Count());
            var timesheet = result.First();
            //Assert.AreEqual(1, timesheet.UserId);
            //Assert.AreEqual("Worked on project", timesheet.Description);
            //Assert.AreEqual(8, timesheet.HoursWorked);
            //Assert.AreEqual(2, timesheet.OverTime);
            //Assert.AreEqual(1, timesheet.ProjectId);
            //Assert.AreEqual(1, timesheet.CategoryId);
            //Assert.AreEqual("Test Project", timesheet.Project.Name);
            //Assert.AreEqual("Test Category", timesheet.Category.Name);
        }

        [Test]
        public async Task GetReportDataAsync_ShouldReturnTimesheetsFilteredByClientId()
        {
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = new Country { Id = 1, Name = "Test Country" }
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
                Category = new Category { Id = 1, Name = "Test Category" }
            };
            var timesheet2 = new Timesheet
            {
                Id = 2,
                UserId = 2,
                User = new User { Id = 2, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" },
                Date = DateTime.UtcNow,
                ProjectId = 2,
                CategoryId = 2,
                HoursWorked = 6,
                Description = "Worked on another project",
                OverTime = 1,
                Project = new Project
                {
                    Id = 2,
                    Name = "Another Project",
                    Description = "Test Description",
                    ClientId = 2,
                    Status = ProjectStatus.Active,
                    Users = new List<User>(),
                    ProjectUsers = new List<ProjectUser>(),
                    Timesheets = new List<Timesheet>()
                },
                Category = new Category { Id = 2, Name = "Another Category" }
            };
            _dbContext.Clients.Add(client);
            _dbContext.Projects.Add(project);
            _dbContext.Timesheets.AddRange(timesheet1, timesheet2);
            await _dbContext.SaveChangesAsync();

            var result = await _reportRepository.GetReportDataAsync(null, 1, null, null, null, null);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().Project.ClientId);
        }

        [Test]
        public async Task GetReportDataAsync_ShouldReturnTimesheetsFilteredByProjectId()
        {
            var client = new Client
            {
                Id = 1,
                Name = "Client1",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1,
                Country = new Country { Id = 1, Name = "Test Country" }
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
                Category = new Category { Id = 1, Name = "Test Category" }
            };
            var timesheet2 = new Timesheet
            {
                Id = 2,
                UserId = 2,
                User = new User { Id = 2, FirstName = "Another User", LastName = "Another LastName", PasswordHash = "hash", PasswordSalt = "salt", Username = "UsernameTest" },
                Date = DateTime.UtcNow,
                ProjectId = 2,
                CategoryId = 2,
                HoursWorked = 6,
                Description = "Worked on another project",
                OverTime = 1,
                Project = new Project
                {
                    Id = 2,
                    Name = "Another Project",
                    Description = "Test Description",
                    ClientId = 2,
                    Status = ProjectStatus.Active,
                    Users = new List<User>(),
                    ProjectUsers = new List<ProjectUser>(),
                    Timesheets = new List<Timesheet>()
                },
                Category = new Category { Id = 2, Name = "Another Category" }
            };
            _dbContext.Projects.Add(project);
            _dbContext.Timesheets.AddRange(timesheet1, timesheet2);
            await _dbContext.SaveChangesAsync();

            var result = await _reportRepository.GetReportDataAsync(null, null, 1, null, null, null);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().ProjectId);
        }

        [Test]
        public async Task GetReportDataAsync_ShouldReturnTimesheetsFilteredByCategoryId()
        {
            var category = new Category { Id = 1, Name = "Test Category" };
            //var category2 = new Category { Id = 2, Name = "Test Category2" };
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
            _dbContext.Categories.AddRange(category);
            _dbContext.Timesheets.AddRange(timesheet1, timesheet2);
            await _dbContext.SaveChangesAsync();

            var result = await _reportRepository.GetReportDataAsync(null, null, null, 1, null, null);

            Assert.AreEqual(2, result.Count());
            //Assert.AreEqual(1, result.First().CategoryId);
        }

        [Test]
        public async Task GetReportDataAsync_ShouldReturnTimesheetsFilteredByDateRange()
        {
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
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 12, 31);
            _dbContext.Timesheets.AddRange(timesheet1, timesheet2);
            await _dbContext.SaveChangesAsync();

            var result = await _reportRepository.GetReportDataAsync(null, null, null, null, startDate, endDate);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(new DateTime(2023, 6, 1), result.First().Date);
        }

        [Test]
        public async Task GetReportDataAsync_ShouldReturnAllTimesheetsWhenNoFiltersApplied()
        {
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
            _dbContext.Timesheets.AddRange(timesheet1, timesheet2);
            await _dbContext.SaveChangesAsync();

            var result = await _reportRepository.GetReportDataAsync(0, 0, 0, 0, null, null);

            Assert.AreEqual(2, result.Count());
        }
    }
}
