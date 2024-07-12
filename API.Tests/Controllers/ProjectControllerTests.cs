using API.Controllers;
using API.RequestEntities;
using Application.RequestEntities;
using Application.Services;
using Application.ViewEntities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;

namespace API.Tests.Controllers
{
    public class ProjectsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public ProjectsControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetProjectsByClientId_ReturnsOkWithProjects_WhenClientIdIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            int validClientId = 1;
            var requestUri = $"/api/Projects/byClientId/{validClientId}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var projects = await response.Content.ReadAsAsync<List<ProjectView>>();
            projects.Should().NotBeNullOrEmpty();
            projects.Should().Contain(p => p.ClientId == validClientId);
        }

        [Fact]
        public async Task GetProjectsByClientId_ReturnsBadRequest_WhenClientIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidClientId = -1;
            var requestUri = $"/api/Projects/byClientId/{invalidClientId}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetProjectsByFilters_ReturnsOkWithProjects_WhenFiltersAreValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var letter = 'P';
            var searchTerm = "Project";
            var requestUri = $"/api/Projects/byFilters?pageNumber={pageNumber}&pageSize={pageSize}&letter={letter}&searchTerm={searchTerm}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var projects = await response.Content.ReadAsAsync<List<ProjectView>>();
            projects.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetProjectsByFilters_ReturnsEmptyList_WhenNoProjectsMatchFilters()
        {
            _factory.ResetDatabase();
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var letter = 'Z';
            var searchTerm = "NoSuchProject";
            var requestUri = $"/api/Projects/byFilters?pageNumber={pageNumber}&pageSize={pageSize}&letter={letter}&searchTerm={searchTerm}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var projects = await response.Content.ReadAsAsync<List<ProjectView>>();
            projects.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var newProject = new ProjectRequest
            {
                Name = "New Project",
                Description = "Project Description",
                ClientId = 1,
                UserId = 1
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(newProject), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Projects", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var createdProject = await response.Content.ReadFromJsonAsync<ProjectView>();
            createdProject.Should().NotBeNull();
            createdProject.Name.Should().Be(newProject.Name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDataIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var invalidProject = new ProjectRequest
            {
                Name = "",
                Description = "Project Description",
                ClientId = 1
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(invalidProject), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Projects", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var updateRequest = new ProjectUpdateRequest
            {
                Id = 1,
                Name = "Updated Project",
                Description = "Updated Description",
                ClientId = 1,
                UserId = 1,
                Status = "Active"
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync("/api/Projects", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedProject = await response.Content.ReadFromJsonAsync<ProjectView>();
            updatedProject.Should().NotBeNull();
            updatedProject.Name.Should().Be(updateRequest.Name);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenDataIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var invalidUpdateRequest = new ProjectUpdateRequest
            {
                Id = -1,
                Name = "Invalid Project",
                Description = "Updated Description",
                Status = "Completed"
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(invalidUpdateRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync("/api/Projects", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var validId = 1;

            // Act
            var response = await _httpClient.DeleteAsync($"/api/Projects/{validId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            // Act
            var response = await _httpClient.DeleteAsync($"/api/Projects/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
