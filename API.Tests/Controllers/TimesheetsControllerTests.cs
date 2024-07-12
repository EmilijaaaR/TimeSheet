using API.RequestEntities;
using Application.RequestEntities;
using Application.ViewEntities;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API.Tests.Controllers
{
    public class TimesheetsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public TimesheetsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;

            var databaseName = $"InMemoryDb-{Guid.NewGuid()}";
            _factory.SetDatabaseName(databaseName);

            _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
        }

        [Fact]
        public async Task Create_ReturnsOk_WithValidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            int userId = 1;
            DateTime date = DateTime.UtcNow;
            var timesheetRequest = new List<TimesheetRequest>
        {
            new TimesheetRequest { ProjectId = 1, CategoryId = 1, HoursWorked = 8, Description = "Worked on project A", OverTime = 2 },
            new TimesheetRequest { ProjectId = 2, CategoryId = 2, HoursWorked = 4, Description = "Worked on project B", OverTime = 1 }
        };

            var requestContent = new StringContent(JsonConvert.SerializeObject(timesheetRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync($"/api/timesheets?userId={userId}&date={date.ToString("o")}", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var timesheet = await response.Content.ReadAsAsync<List<TimesheetSummary>>();
            timesheet.Should().NotBeNull();
            timesheet.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WithInvalidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            DateTime date = DateTime.UtcNow;
            var invalidTimesheetRequest = new List<TimesheetRequest>();

            var requestContent = new StringContent(JsonConvert.SerializeObject(invalidTimesheetRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync($"/api/timesheets?userId=&date={date.ToString("o")}", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WithValidId()
        {
            _factory.ResetDatabase();
            // Arrange
            int validId = 1;

            // Act
            var response = await _httpClient.DeleteAsync($"/api/timesheets?id={validId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WithInvalidId()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            // Act
            var response = await _httpClient.DeleteAsync($"/api/timesheets?id={invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetTimesheetsWithStatus_ReturnsOk_WithValidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow;

            // Act
            var response = await _httpClient.GetAsync($"/api/timesheets/status?userId={userId}&startDate={startDate.ToString("o")}&endDate={endDate.ToString("o")}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var summary = await response.Content.ReadAsAsync<TimesheetSummary>();
            summary.Should().NotBeNull();
            summary.TimesheetResults.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetTimesheetsWithStatus_ReturnsBadRequest_WithInvalidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            int userId = 9;
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = DateTime.UtcNow.AddDays(-30); // Invalid because endDate is before startDate

            // Act
            var response = await _httpClient.GetAsync($"/api/timesheets/status?userId={userId}&startDate={startDate.ToString("o")}&endDate={endDate.ToString("o")}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateTimesheetsWithStatus_ReturnsOk_WithValidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            int userId = 2;
            DateTime startDate = new DateTime(2024, 6, 20);
            DateTime endDate = new DateTime(2024, 6, 30);
            var timesheetUpdateRequest = new List<TimesheetUpdateRequest>
            {
            new TimesheetUpdateRequest { Id = 2, ProjectId = 1, CategoryId = 1, HoursWorked = 8, Description = "Updated work on project A", OverTime = 2 }
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(timesheetUpdateRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync($"/api/timesheets/update?userId={userId}&startDate={startDate.ToString("o")}&endDate={endDate.ToString("o")}", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsAsync<List<TimesheetSummary>>();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateTimesheetsWithStatus_ReturnsBadRequest_WithInvalidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            int userId = 1;
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = DateTime.UtcNow.AddDays(-30); // Invalid because endDate is before startDate
            var timesheetUpdateRequest = new List<TimesheetUpdateRequest>
        {
            new TimesheetUpdateRequest { Id = 1, ProjectId = 1, CategoryId = 1, HoursWorked = 8, Description = "Updated work on project A", OverTime = 2 }
        };

            var requestContent = new StringContent(JsonConvert.SerializeObject(timesheetUpdateRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync($"/api/timesheets/update?userId={userId}&startDate={startDate.ToString("o")}&endDate={endDate.ToString("o")}", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
