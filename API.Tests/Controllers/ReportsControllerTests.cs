using Application.RequestEntities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Application.ViewEntities;

namespace API.Tests.Controllers
{
    public class ReportsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public ReportsControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GenerateReportAsync_ReturnsOk_WhenRequestIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var reportRequest = new ReportRequest
            {
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                ProjectId = 1
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(reportRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/reports/generate", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var reportData = await response.Content.ReadAsAsync<List<ReportView>>();
            reportData.Should().NotBeNull();
            reportData.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GenerateReportAsync_ReturnsEmptyRequest_WhenRequestIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var invalidReportRequest = new ReportRequest
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow,
                ProjectId = 1
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(invalidReportRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/reports/generate", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var reportData = await response.Content.ReadAsAsync<List<ReportView>>();
            reportData.Should().BeEmpty();
        }
    }
}
