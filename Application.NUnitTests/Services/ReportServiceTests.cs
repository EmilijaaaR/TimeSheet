using Moq;
using Domain.Entities;
using Domain.Repositories;
using Application.Services;
using Application.RequestEntities;
using NUnit.Framework.Legacy;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class ReportServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private ReportService _reportService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _reportService = new ReportService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task GenerateReportAsync_ShouldReturnReportViews()
        {
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1;
            // Arrange
            var reportRequest = new ReportRequest
            {
                UserId = 1,
                ClientId = 1,
                ProjectId = 1,
                CategoryId = 1,
                StartDate = DateTime.Now.Date.AddDays(-7),
                EndDate = DateTime.Now.Date
            };

            var timesheets = new List<Timesheet>
            {
                new Timesheet
                {
                    UserId = 1,
                    ProjectId = 1,
                    CategoryId = 1,
                    Date = DateTime.Now.Date,
                    HoursWorked = 8,
                    User = user,
                    Project = new Project { Name = "Project A" },
                    Category = new Category { Name = "Development" }
                },
                new Timesheet
                {
                    UserId = 1,
                    ProjectId = 2,
                    CategoryId = 2,
                    Date = DateTime.Now.Date.AddDays(-1),
                    HoursWorked = 6,
                    User = user,
                    Project = new Project { Name = "Project B" },
                    Category = new Category { Name = "Testing" }
                }
            };

            _mockUnitOfWork.Setup(u => u.ReportRepository.GetReportDataAsync(
                reportRequest.UserId,
                reportRequest.ClientId,
                reportRequest.ProjectId,
                reportRequest.CategoryId,
                reportRequest.StartDate,
                reportRequest.EndDate))
                .ReturnsAsync(timesheets);

            // Act
            var result = await _reportService.GenerateReportAsync(reportRequest);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(timesheets.Count, result.Count());

            var firstTimesheet = timesheets.First();
            var firstResult = result.First();
            ClassicAssert.AreEqual(firstTimesheet.User.FirstName + " " + firstTimesheet.User.LastName, firstResult.UserName);
            ClassicAssert.AreEqual(firstTimesheet.Project.Name, firstResult.ProjectName);
            ClassicAssert.AreEqual(firstTimesheet.Category.Name, firstResult.CategoryName);
            ClassicAssert.AreEqual(firstTimesheet.Date, firstResult.Date);
            ClassicAssert.AreEqual(firstTimesheet.HoursWorked + firstTimesheet.OverTime, firstResult.Time);

        }

        [Test]
        public async Task GenerateReportAsync_ShouldReturnEmpty_WhenNoDataFound()
        {
            // Arrange
            var reportRequest = new ReportRequest
            {
                UserId = 1,
                ClientId = 1,
                ProjectId = 1,
                CategoryId = 1,
                StartDate = DateTime.Now.Date.AddDays(-7),
                EndDate = DateTime.Now.Date
            };

            _mockUnitOfWork.Setup(u => u.ReportRepository.GetReportDataAsync(
                reportRequest.UserId,
                reportRequest.ClientId,
                reportRequest.ProjectId,
            reportRequest.CategoryId,
                reportRequest.StartDate,
                reportRequest.EndDate))
                .ReturnsAsync(new List<Timesheet>());

            // Act
            var result = await _reportService.GenerateReportAsync(reportRequest);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsEmpty(result);
        }

        [Test]
        public void GenerateReportAsync_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            var reportRequest = new ReportRequest();

            _mockUnitOfWork.Setup(u => u.ReportRepository.GetReportDataAsync(
                reportRequest.UserId,
                reportRequest.ClientId,
                reportRequest.ProjectId,
                reportRequest.CategoryId,
                reportRequest.StartDate,
                reportRequest.EndDate))
                .ThrowsAsync(new Exception("Repository exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _reportService.GenerateReportAsync(reportRequest));
        }
    }
}
