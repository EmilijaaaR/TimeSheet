using Application.RequestEntities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API.Tests.Controllers
{
    public class GeneratePdfControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public GeneratePdfControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GeneratePdfReportAsync_ReturnsOk_WithValidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            var request = new ReportRequest
            {
                UserId = 1,
                ClientId = 1,
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                CategoryId = 1,
                ProjectId = 1
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/GeneratePdf/generate", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/pdf");
            response.Content.Headers.ContentDisposition.FileName.Should().Be("report.pdf");

            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            fileBytes.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GeneratePdfReportAsync_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var invalidRequest = new ReportRequest
            {
                UserId = 0,
                ClientId = 0,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(-30),
                CategoryId = 0,
                ProjectId = 0
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(invalidRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/GeneratePdf/generate", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GeneratePdfReportAsync_ReturnsBadRequest_WhenRequestIsMissing()
        {
            _factory.ResetDatabase();
            // Arrange
            var requestContent = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/GeneratePdf/generate", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
