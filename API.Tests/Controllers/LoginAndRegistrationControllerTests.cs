using Application.ViewEntities;
using Application.RequestEntities;
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
    public class LoginAndRegistrationControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public LoginAndRegistrationControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task Login_ReturnsOk_WithValidCredentials()
        {
            _factory.ResetDatabase();
            // Arrange
            var userRequest = new UserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "newuser",
                Password = "NewUserPassword123"
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("/api/LoginAndRegistration/register", requestContent);

            var requestUri = $"/api/LoginAndRegistration/login?username={userRequest.Username}&password={userRequest.Password}";
            var response = await _httpClient.PostAsync(requestUri, null);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var token = await response.Content.ReadAsStringAsync();
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WithInvalidCredentials()
        {
            _factory.ResetDatabase();
            // Arrange
            var username = "invaliduser";
            var password = "InvalidPassword";
            var requestUri = $"/api/LoginAndRegistration/login?username={username}&password={password}";

            // Act
            var response = await _httpClient.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Register_ReturnsOk_WithValidRequest()
        {
            _factory.ResetDatabase();
            // Arrange
            var userRequest = new UserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "newuser",
                Password = "NewUserPassword123"
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/LoginAndRegistration/register", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var userView = await response.Content.ReadFromJsonAsync<UserView>();
            userView.Should().NotBeNull();
            userView.Username.Should().Be(userRequest.Username);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRequestIsMissing()
        {
            _factory.ResetDatabase();
            // Arrange
            var requestContent = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/LoginAndRegistration/register", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

}
