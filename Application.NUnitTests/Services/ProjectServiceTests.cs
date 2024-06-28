using Moq;
using Domain.Entities;
using Domain.Repositories;
using Application.Services;
using Application.Enums;
using Application.MappingExtension;
using API.RequestEntities;
using NUnit.Framework.Legacy;
using Application.RequestEntities;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class ProjectServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private ProjectService _projectService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _projectService = new ProjectService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task CreateAsync_ValidRequest_ReturnsProjectView()
        {
            // Arrange
            var request = new ProjectRequest { Name = "Test Project", Description = "Description", ClientId = 1, UserId = 1 };
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(request.ClientId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(request.UserId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.InsertAsync(It.IsAny<Project>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _projectService.CreateAsync(request);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(request.Name, result.Name);
        }

        [Test]
        public void CreateAsync_ClientDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var request = new ProjectRequest { Name = "Test Project", Description = "Description", ClientId = 1, UserId = 1 };
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(request.ClientId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.CreateAsync(request));
        }

        [Test]
        public void CreateAsync_UserDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var request = new ProjectRequest { Name = "Test Project", Description = "Description", ClientId = 1, UserId = 1 };
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(request.ClientId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(request.UserId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.CreateAsync(request));
        }

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsProjectView()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId, Name = "Test Project" };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await _projectService.GetByIdAsync(projectId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(projectId, result.Id);
        }

        [Test]
        public void GetByIdAsync_InvalidId_ThrowsArgumentException()
        {
            // Arrange
            var projectId = 1;
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.GetByIdAsync(projectId));
        }

        [Test]
        public async Task UpdateAsync_ValidRequest_ReturnsUpdatedProjectView()
        {
            // Arrange
            var request = new ProjectUpdateRequest { Id = 1, Name = "Updated Project", Description = "Updated Description", ClientId = 1, UserId = 1, Status = "Active" };
            var existingProject = new Project { Id = request.Id, Name = "Test Project", Description = "Description", ClientId = 1, Status = ProjectStatus.Active.ToDomainStatus()};
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(request.Id)).ReturnsAsync(existingProject);
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(request.ClientId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(request.UserId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByProjectIdAsync(request.Id)).ReturnsAsync((ProjectUser)null);
            _mockUnitOfWork.Setup(u => u.UserRepository.AssignLeadToProjectAsync(It.IsAny<ProjectUser>()));
            _mockUnitOfWork.Setup(u => u.UserRepository.DeleteLeadToProjectAsync(It.IsAny<ProjectUser>()));

            // Act
            var result = await _projectService.UpdateAsync(request);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(request.Name, result.Name);
        }

        [Test]
        public void UpdateAsync_ProjectNotFound_ThrowsArgumentException()
        {
            // Arrange
            var request = new ProjectUpdateRequest { Id = 1, Name = "Updated Project", Description = "Updated Description", ClientId = 1, UserId = 1, Status = "Active" };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(request.Id)).ReturnsAsync((Project)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.UpdateAsync(request));
        }

        [Test]
        public void UpdateAsync_ClientDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var request = new ProjectUpdateRequest { Id = 1, Name = "Updated Project", Description = "Updated Description", ClientId = 1, UserId = 1, Status = "Active" };
            var existingProject = new Project { Id = request.Id, Name = "Test Project", Description = "Description", ClientId = 1, Status = ProjectStatus.Active.ToDomainStatus() };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(request.Id)).ReturnsAsync(existingProject);
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(request.ClientId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.UpdateAsync(request));
        }

        [Test]
        public void UpdateAsync_UserDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var request = new ProjectUpdateRequest { Id = 1, Name = "Updated Project", Description = "Updated Description", ClientId = 1, UserId = 1, Status = "Active" };
            var existingProject = new Project { Id = request.Id, Name = "Test Project", Description = "Description", ClientId = 1, Status = ProjectStatus.Active.ToDomainStatus() };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(request.Id)).ReturnsAsync(existingProject);
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(request.ClientId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(request.UserId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.UpdateAsync(request));
        }

        [Test]
        public void UpdateAsync_InvalidStatus_ThrowsArgumentException()
        {
            // Arrange
            var request = new ProjectUpdateRequest { Id = 1, Name = "Updated Project", Description = "Updated Description", ClientId = 1, UserId = 1, Status = "InvalidStatus" };
            var existingProject = new Project { Id = request.Id, Name = "Test Project", Description = "Description", ClientId = 1, Status = ProjectStatus.Active.ToDomainStatus() };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(request.Id)).ReturnsAsync(existingProject);
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(request.ClientId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(request.UserId)).ReturnsAsync(true);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.UpdateAsync(request));
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesProject()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId, Name = "Test Project" };
            var projectUser = new ProjectUser { ProjectId = projectId, UserId = 1 };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByProjectIdAsync(projectId)).ReturnsAsync(projectUser);
            _mockUnitOfWork.Setup(u => u.UserRepository.DeleteLeadToProjectAsync(projectUser));
            _mockUnitOfWork.Setup(u => u.ProjectRepository.DeleteAsync(project));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _projectService.DeleteAsync(projectId);

            // Assert
            _mockUnitOfWork.Verify(u => u.ProjectRepository.DeleteAsync(project), Times.Once);
        }

        [Test]
        public void DeleteAsync_ProjectNotFound_ThrowsArgumentException()
        {
            // Arrange
            var projectId = 1;
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.DeleteAsync(projectId));
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project1" },
                new Project { Id = 2, Name = "Project2" }
            };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetAllAsync()).ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetAllAsync();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetProjectsByClientIdAsync_ValidClientId_ReturnsProjects()
        {
            // Arrange
            var clientId = 1;
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project1", ClientId = clientId },
                new Project { Id = 2, Name = "Project2", ClientId = clientId }
            };
            var projectUsers = new List<ProjectUser>
            {
                new ProjectUser { ProjectId = 1, UserId = 1 },
                new ProjectUser { ProjectId = 2, UserId = 2 }
            };
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(clientId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByClientIdAsync(clientId)).ReturnsAsync(projects);
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByProjectsIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(projectUsers);

            // Act
            var result = await _projectService.GetProjectsByClientIdAsync(clientId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetProjectsByClientIdAsync_ClientDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var clientId = 1;
            _mockUnitOfWork.Setup(u => u.ClientRepository.ExistsAsync(clientId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _projectService.GetProjectsByClientIdAsync(clientId));
        }

        [Test]
        public async Task GetProjectsByFiltersAsync_ValidFilters_ReturnsFilteredProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project1" },
                new Project { Id = 2, Name = "Project2" }
            };
            var projectUsers = new List<ProjectUser>
            {
                new ProjectUser { ProjectId = 1, UserId = 1 },
                new ProjectUser { ProjectId = 2, UserId = 2 }
            };
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByFiltersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<char?>(), It.IsAny<string>())).ReturnsAsync(projects);
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByProjectsIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(projectUsers);

            // Act
            var result = await _projectService.GetProjectsByFiltersAsync(1, 10, 'P', "Project");

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }
    }
}
