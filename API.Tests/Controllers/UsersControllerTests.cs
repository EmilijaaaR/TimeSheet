using Application.RequestEntities;
using Application.ViewEntities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;


namespace API.Tests.Controllers
{
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetAll_ReturnsOkWithUsers_WhenCalledWithValidRole()
        {
            _factory.ResetDatabase();
            var response = await _httpClient.GetAsync("/api/users");

            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadAsAsync<List<UserView>>();
            users.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetById_ReturnsOkWithUser_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            var userRequest = new UserRequest
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Password = "Test@123"
            };
            var content = new StringContent(JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json");

            var responseCreate = await _httpClient.PostAsync("/api/users", content);

            responseCreate.EnsureSuccessStatusCode();
            var createdUser = await responseCreate.Content.ReadFromJsonAsync<UserView>();
            int validId = createdUser.Id;
            var response = await _httpClient.GetAsync($"/api/users/{validId}");

            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<UserView>();
            user.Should().NotBeNull();
            user.Id.Should().Be(validId);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            var response = await _httpClient.GetAsync("/api/users/-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ReturnsOkWithCreatedUser_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            var userRequest = new UserRequest
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser1",
                Password = "Test@1234"
            };
            var content = new StringContent(JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/users", content);

            response.EnsureSuccessStatusCode();
            var createdUser = await response.Content.ReadFromJsonAsync<UserView>();
            createdUser.Should().NotBeNull();
            createdUser.FirstName.Should().Be(userRequest.FirstName);
            createdUser.LastName.Should().Be(userRequest.LastName);
            createdUser.Username.Should().Be(userRequest.Username);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDataIsInvalid()
        {
            _factory.ResetDatabase();
            var userRequest = new UserRequest
            {
                LastName = "",
                Username = "invaliduser",
                Password = "Test@123"
            };
            var content = new StringContent(JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/users", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_ReturnsOkWithUpdatedUser_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            var userRequest = new UserRequest
            {
                FirstName = "Updated",
                LastName = "User",
                Username = "updateduser",
                Password = "Updated@123"
            };
            var content = new StringContent(JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("/api/users/2", content);

            response.EnsureSuccessStatusCode();
            var updatedUser = await response.Content.ReadFromJsonAsync<UserView>();
            updatedUser.Should().NotBeNull();
            updatedUser.FirstName.Should().Be(userRequest.FirstName);
            updatedUser.LastName.Should().Be(userRequest.LastName);
            updatedUser.Username.Should().Be(userRequest.Username);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenDataIsInvalid()
        {
            _factory.ResetDatabase();
            var userRequest = new UserRequest
            {
                FirstName = "",
                LastName = "",
                Username = "invaliduser",
                Password = "Updated@123"
            };
            var content = new StringContent(JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("/api/users/-1", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            var response = await _httpClient.DeleteAsync("/api/users/1");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            var response = await _httpClient.DeleteAsync("/api/users/-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

}
