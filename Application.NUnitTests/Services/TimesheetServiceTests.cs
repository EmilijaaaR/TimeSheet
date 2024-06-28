using API.RequestEntities;
using Application.RequestEntities;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using NUnit.Framework.Legacy;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class TimesheetServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private TimesheetService _timesheetService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _timesheetService = new TimesheetService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateTimesheets()
        {
            // Arrange
            int userId = 1;
            DateTime date = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetRequest>
            {
                new TimesheetRequest { ProjectId = 1, CategoryId = 1, HoursWorked = 8, Description = "Desc", OverTime = 2 }
            };

                    var projects = new List<Project>
            {
                new Project { Id = 1, ClientId = 1, Name = "Project1" },
                new Project { Id = 2, ClientId = 2, Name = "Project2" }
            };

                    var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(projects);
            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetCategoriesByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(categories);

            _mockUnitOfWork.Setup(u => u.TimesheetRepository.InsertAsync(It.IsAny<IEnumerable<Timesheet>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _timesheetService.CreateAsync(userId, date, timesheetRequests);

            // Assert
            _mockUnitOfWork.Verify(u => u.TimesheetRepository.InsertAsync(It.IsAny<IEnumerable<Timesheet>>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(timesheetRequests.Count, result.Count());

            // Additional assertions to check the content of the returned TimesheetViews
            ClassicAssert.AreEqual(1, result.First().ClientId);
            ClassicAssert.AreEqual(1, result.First().ProjectId);
            ClassicAssert.AreEqual(1, result.First().CategoryId);
            ClassicAssert.AreEqual("Desc", result.First().Description);
            ClassicAssert.AreEqual(8, result.First().HoursWorked);
            ClassicAssert.AreEqual(2, result.First().OverTime);
        }


        [Test]
        public void CreateAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime date = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetRequest>();

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(false);

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.CreateAsync(userId, date, timesheetRequests));
            ClassicAssert.AreEqual("User does not exist.", ex.Message);
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime date = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetRequest>
        {
            new TimesheetRequest { ProjectId = 1, CategoryId = 1, HoursWorked = 8, Description = "Desc", OverTime = 2 }
        };

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Project>());
            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetCategoriesByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category>
                {
                new Category { Id = 1, Name = "Category1" }
                });
            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.CreateAsync(userId, date, timesheetRequests));
            ClassicAssert.AreEqual("Project does not exist.", ex.Message);
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime date = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetRequest>
        {
            new TimesheetRequest { ProjectId = 1, CategoryId = 1, HoursWorked = 8 }
        };

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Project>
                {
                new Project { Id = 1, ClientId = 1, Name = "Project1" }
                });
            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetCategoriesByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category>());

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.CreateAsync(userId, date, timesheetRequests));
            ClassicAssert.AreEqual("Category does not exist.", ex.Message);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteTimesheet()
        {
            // Arrange
            int id = 1;
            var timesheet = new Timesheet { Id = id };

            _mockUnitOfWork.Setup(u => u.TimesheetRepository.GetByIdAsync(id)).ReturnsAsync(timesheet);

            // Act
            await _timesheetService.DeleteAsync(id);

            // Assert
            _mockUnitOfWork.Verify(u => u.TimesheetRepository.DeleteAsync(timesheet), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_WhenTimesheetDoesNotExist()
        {
            // Arrange
            int id = 1;
            _mockUnitOfWork.Setup(u => u.TimesheetRepository.GetByIdAsync(id)).ReturnsAsync((Timesheet)null);

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.DeleteAsync(id));
            ClassicAssert.AreEqual("Timesheet does not exist.", ex.Message);
        }

        [Test]
        public async Task GetTimesheetsWithStatusAsync_ShouldReturnTimesheetSummary()
        {
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.Now.Date.AddDays(-7);
            DateTime endDate = DateTime.Now.Date;
            var timesheets = new List<Timesheet>
        {
            new Timesheet
            {
                UserId = userId,
                Project = new Project { Id = 1, ClientId = 1, Name = "Project1" },
                Category = new Category { Id = 1, Name = "Category1" },
                Date = DateTime.Now.Date,
                HoursWorked = 8,
                OverTime = 2,
                Description = "Desc"
            },
            new Timesheet
            {
                UserId = userId,
                Project = new Project { Id = 2, ClientId = 2, Name = "Project2" },
                Category = new Category { Id = 2, Name = "Category2" },
                Date = DateTime.Now.Date.AddDays(-1),
                HoursWorked = 6,
                OverTime = 1,
                Description = "Desc2"
            }
        };

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.TimesheetRepository.GetTimesheetsAsync(userId, startDate, endDate)).ReturnsAsync(timesheets);

            // Act
            var result = await _timesheetService.GetTimesheetsWithStatusAsync(userId, startDate, endDate);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, result.TimesheetResults.Count);
            ClassicAssert.AreEqual(timesheets.Sum(t => t.HoursWorked + t.OverTime), result.TotalHoursAll);
        }

        [Test]
        public void GetTimesheetsWithStatusAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.Now.Date.AddDays(-7);
            DateTime endDate = DateTime.Now.Date;

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(false);

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.GetTimesheetsWithStatusAsync(userId, startDate, endDate));
            ClassicAssert.AreEqual("User does not exist.", ex.Message);
        }

        [Test]
        public async Task UpdateTimesheetsWithStatusAsync_ShouldUpdateTimesheets()
        {
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.Now.Date.AddDays(-7);
            DateTime endDate = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetUpdateRequest>
        {
            new TimesheetUpdateRequest { Id = 1, ProjectId = 1, CategoryId = 1, HoursWorked = 8, OverTime = 2, Description = "UpdatedDesc" }
        };
            var existingTimesheets = new List<Timesheet>
        {
            new Timesheet
            {
                Id = 1,
                UserId = userId,
                Project = new Project { Id = 1, ClientId = 1, Name = "Project1" },
                Category = new Category { Id = 1, Name = "Category1" },
                Date = DateTime.Now.Date.AddDays(-2),
                HoursWorked = 5,
                OverTime = 1,
                Description = "Desc"
            }
        };

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.TimesheetRepository.GetTimesheetsAsync(userId, startDate, endDate)).ReturnsAsync(existingTimesheets);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Project>
                {
                new Project { Id = 1, ClientId = 1, Name = "Project1" }
                });
            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetCategoriesByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category>
                {
                new Category { Id = 1, Name = "Category1" }
                });

            // Act
            var result = await _timesheetService.UpdateTimesheetsWithStatusAsync(userId, startDate, endDate, timesheetRequests);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(1, result.Count());
            ClassicAssert.AreEqual("UpdatedDesc", result.First().Description);
        }

        [Test]
        public void UpdateTimesheetsWithStatusAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.Now.Date.AddDays(-7);
            DateTime endDate = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetUpdateRequest>();

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(false);

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.UpdateTimesheetsWithStatusAsync(userId, startDate, endDate, timesheetRequests));
            ClassicAssert.AreEqual("User does not exist.", ex.Message);
        }

        [Test]
        public void UpdateTimesheetsWithStatusAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.Now.Date.AddDays(-7);
            DateTime endDate = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetUpdateRequest>
        {
            new TimesheetUpdateRequest { Id = 1, ProjectId = 1, CategoryId = 1, HoursWorked = 8, OverTime = 2 }
        };
            var existingTimesheets = new List<Timesheet>
        {
            new Timesheet
            {
                Id = 1,
                UserId = userId,
                Project = new Project { Id = 1, ClientId = 1, Name = "Project1" },
                Category = new Category { Id = 1, Name = "Category1" },
                Date = DateTime.Now.Date.AddDays(-2),
                HoursWorked = 5,
                OverTime = 1,
                Description = "Desc"
            }
        };

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.TimesheetRepository.GetTimesheetsAsync(userId, startDate, endDate)).ReturnsAsync(existingTimesheets);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Project>());
            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetCategoriesByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category>
                {
                            new Category { Id = 1, Name = "Category1" }
                });

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.UpdateTimesheetsWithStatusAsync(userId, startDate, endDate, timesheetRequests));
            ClassicAssert.AreEqual("Project does not exist.", ex.Message);
        }

        [Test]
        public void UpdateTimesheetsWithStatusAsync_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.Now.Date.AddDays(-7);
            DateTime endDate = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetUpdateRequest>
        {
            new TimesheetUpdateRequest { Id = 1, ProjectId = 1, CategoryId = 1, HoursWorked = 8, OverTime = 2 }
        };
            var existingTimesheets = new List<Timesheet>
        {
            new Timesheet
            {
                Id = 1,
                UserId = userId,
                Project = new Project { Id = 1, ClientId = 1, Name = "Project1" },
                Category = new Category { Id = 1, Name = "Category1" },
                Date = DateTime.Now.Date.AddDays(-2),
                HoursWorked = 5,
                OverTime = 1,
                Description = "Desc"
            }
        };

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.TimesheetRepository.GetTimesheetsAsync(userId, startDate, endDate)).ReturnsAsync(existingTimesheets);
            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetCategoriesByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category>());
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Project>
                {
                            new Project { Id = 1, ClientId = 1, Name = "Project1" }
                });

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.UpdateTimesheetsWithStatusAsync(userId, startDate, endDate, timesheetRequests));
            ClassicAssert.AreEqual("Category does not exist.", ex.Message);
        }

        [Test]
        public void UpdateTimesheetsWithStatusAsync_ShouldThrowException_WhenTimesheetDoesNotExist()
        {
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.Now.Date.AddDays(-7);
            DateTime endDate = DateTime.Now.Date;
            var timesheetRequests = new List<TimesheetUpdateRequest>
        {
            new TimesheetUpdateRequest { Id = 1, ProjectId = 1, CategoryId = 1, HoursWorked = 8, OverTime = 2 }
        };
            var existingTimesheets = new List<Timesheet>();

            _mockUnitOfWork.Setup(u => u.UserRepository.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.TimesheetRepository.GetTimesheetsAsync(userId, startDate, endDate)).ReturnsAsync(existingTimesheets);
            _mockUnitOfWork.Setup(u => u.ProjectRepository.GetProjectsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Project>
                {
                new Project { Id = 1, ClientId = 1, Name = "Project1" }
                });
            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetCategoriesByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category>
                {
                new Category { Id = 1, Name = "Category1" }
                });

            // Act & Assert
            var ex = ClassicAssert.ThrowsAsync<ArgumentException>(async () => await _timesheetService.UpdateTimesheetsWithStatusAsync(userId, startDate, endDate, timesheetRequests));
            ClassicAssert.AreEqual("Timesheet does not exist.", ex.Message);
        }
    }

}
