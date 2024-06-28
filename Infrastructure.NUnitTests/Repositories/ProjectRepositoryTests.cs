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
    public class ProjectRepositoryTests
    {
        private TimeSheetDbContext _dbContext;
        private IProjectRepository _projectRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TimeSheetDbContext(options);
            _projectRepository = new ProjectRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldAddProjectToDatabase()
        {
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 1,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };

            await _projectRepository.InsertAsync(project);
            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Projects.FindAsync(project.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(project.Name, result.Name);
            Assert.AreEqual(project.Description, result.Description);
            Assert.AreEqual(project.ClientId, result.ClientId);
            Assert.AreEqual(project.Status, result.Status);
            Assert.IsEmpty(result.Users);
            Assert.IsEmpty(result.ProjectUsers);
            Assert.IsEmpty(result.Timesheets);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnProjectWithAllProperties()
        {
            var client = new Client 
            { 
                Id = 1, 
                Name = "Test Client",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1
            };
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Description = "Test Description",
                Client = client,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser> { new ProjectUser { ProjectId = 1, UserId = 1 } },
                Timesheets = new List<Timesheet>()
            };
            _dbContext.Clients.Add(client);
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            var result = await _projectRepository.GetByIdAsync(project.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(project.Name, result.Name);
            Assert.AreEqual(project.Description, result.Description);
            Assert.IsNotNull(result.Client);
            Assert.AreEqual(client.Name, result.Client.Name);
            Assert.AreEqual(project.Status, result.Status);
            Assert.AreEqual(0, result.Users.Count);
            Assert.AreEqual(1, result.ProjectUsers.Count);
            Assert.AreEqual(0, result.Timesheets.Count);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveProjectFromDatabase()
        {
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Description = "Test Description",
                ClientId = 1,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            await _projectRepository.DeleteAsync(project);
            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Projects.FindAsync(project.Id);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllProjectsWithAllProperties()
        {
            var client = new Client 
            { 
                Id = 1, 
                Name = "Test Client",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1
            };
            var project1 = new Project
            {
                Id = 1,
                Name = "Test Project 1",
                Description = "Test Description 1",
                Client = client,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Test Project 2",
                Description = "Test Description 2",
                Client = client,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            _dbContext.Clients.Add(client);
            _dbContext.Projects.AddRange(project1, project2);
            await _dbContext.SaveChangesAsync();

            var result = await _projectRepository.GetAllAsync();
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual("Test Project 1", resultList.First().Name);
            Assert.AreEqual("Test Description 1", resultList.First().Description);
            Assert.AreEqual(ProjectStatus.Active, resultList.First().Status);
            Assert.AreEqual(0, resultList.First().Timesheets.Count);
        }

        [Test]
        public async Task GetProjectsByClientIdAsync_ShouldReturnProjectsForClient()
        {
            var client = new Client
            {
                Id = 1,
                Name = "Test Client",
                Address = "123 Main St",
                City = "City",
                PostalCode = "12345",
                CountryId = 1
            };
            var project1 = new Project
            {
                Id = 1,
                Name = "Test Project 1",
                Description = "Test Description 1",
                ClientId = client.Id,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Test Project 2",
                Description = "Test Description 2",
                ClientId = client.Id,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            _dbContext.Clients.Add(client);
            _dbContext.Projects.AddRange(project1, project2);
            await _dbContext.SaveChangesAsync();

            var result = await _projectRepository.GetProjectsByClientIdAsync(client.Id);
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);
            Assert.IsTrue(resultList.All(p => p.ClientId == client.Id));
        }

        [Test]
        public async Task GetProjectsByIdsAsync_ShouldReturnProjectsWithSpecifiedIds()
        {
            var project1 = new Project
            {
                Id = 1,
                Name = "Test Project 1",
                Description = "Test Description 1",
                ClientId = 1,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            var project2 = new Project
            {
                Id = 2,
                Name = "Test Project 2",
                Description = "Test Description 2",
                ClientId = 2,
                Status = ProjectStatus.Active,
                Users = new List<User>(),
                ProjectUsers = new List<ProjectUser>(),
                Timesheets = new List<Timesheet>()
            };
            _dbContext.Projects.AddRange(project1, project2);
            await _dbContext.SaveChangesAsync();

            var result = await _projectRepository.GetProjectsByIdsAsync(new List<int> { project1.Id, project2.Id });
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);
            Assert.IsTrue(resultList.Any(p => p.Id == project1.Id));
            Assert.IsTrue(resultList.Any(p => p.Id == project2.Id));
        }

        [Test]
        public async Task GetProjectsByFiltersAsync_ShouldReturnPagedProjects()
        {
            var project1 = new Project { Id = 1, Name = "Alpha Project", Description = "Desc1", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            var project2 = new Project { Id = 2, Name = "Beta Project", Description = "Desc2", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            var project3 = new Project { Id = 3, Name = "Gamma Project", Description = "Desc3", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            _dbContext.Projects.AddRange(project1, project2, project3);
            await _dbContext.SaveChangesAsync();

            var result = await _projectRepository.GetProjectsByFiltersAsync(1, 2, null, null);
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual("Alpha Project", resultList.First().Name);
        }

        [Test]
        public async Task GetProjectsByFiltersAsync_ShouldReturnFilteredProjectsByLetter()
        {
            var project1 = new Project { Id = 1, Name = "Alpha Project", Description = "Desc1", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            var project2 = new Project { Id = 2, Name = "Beta Project", Description = "Desc2", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            var project3 = new Project { Id = 3, Name = "Gamma Project", Description = "Desc3", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            _dbContext.Projects.AddRange(project1, project2, project3);
            await _dbContext.SaveChangesAsync();

            var result = await _projectRepository.GetProjectsByFiltersAsync(1, 10, 'A', null);
            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("Alpha Project", resultList.First().Name);
        }

        [Test]
        public async Task GetProjectsByFiltersAsync_ShouldReturnFilteredProjectsBySearchTerm()
        {
            var project1 = new Project { Id = 1, Name = "Alpha Project", Description = "Desc1", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            var project2 = new Project { Id = 2, Name = "Beta Project", Description = "Desc2", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            var project3 = new Project { Id = 3, Name = "Gamma Project", Description = "Desc3", ClientId = 1, Status = ProjectStatus.Active, Users = new List<User>(), ProjectUsers = new List<ProjectUser>(), Timesheets = new List<Timesheet>() };
            _dbContext.Projects.AddRange(project1, project2, project3);
            await _dbContext.SaveChangesAsync();

            var result = await _projectRepository.GetProjectsByFiltersAsync(1, 10, null, "Beta");
            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("Beta Project", resultList.First().Name);
        }
    }
}
