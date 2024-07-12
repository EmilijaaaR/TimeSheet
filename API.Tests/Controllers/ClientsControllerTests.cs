using API.RequestEntities;
using API.ViewEntities;
using Application.ViewEntities;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace API.Tests.Controllers
{
    public class ClientsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public ClientsControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetAll_ReturnsOkWithClients_WhenCalledWithValidRole()
        {
            _factory.ResetDatabase();
            var response = await _httpClient.GetAsync("/api/clients");

            // Assert
            response.EnsureSuccessStatusCode();
            var clients = await response.Content.ReadAsAsync<List<ClientView>>();
            clients.Should().NotBeNullOrEmpty();
            Assert.NotNull(clients);
            Assert.True(clients.Any());
        }

        [Fact]
        public async Task GetById_ReturnsOkWithClient_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            // Act
            var response = await _httpClient.GetAsync("/api/Clients/4");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var client = await response.Content.ReadFromJsonAsync<ClientView>();

            client.Should().NotBeNull();
            client.Id.Should().Be(4);
            client.Name.Should().Be("Client 4");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            var requestUri = $"/api/Clients/{invalidId}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetClientsByFilters_ReturnsOk_WhenFiltersAreValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var query = "?pageNumber=1&pageSize=10&letter=C&searchTerm=Client 2";
            var requestUri = $"/api/clients/byFilters{query}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            var clients = await response.Content.ReadAsAsync<List<ClientView>>();
            clients.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetClientsByFilters_ReturnsBadRequest_WhenFiltersAreInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var query = "?pageNumber=1&pageSize=10&letter=C&searchTerm=Clll";
            var requestUri = $"/api/clients/byFilters{query}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var clients = await response.Content.ReadAsAsync<List<ClientView>>();
            clients.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var createClient = new
            {
                name = "Created Client",
                address = "Created Address",
                city = "Created City",
                postalCode = "Created Postal Code",
                countryId = 1
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(createClient), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync($"/api/Clients", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var createdClient = await response.Content.ReadFromJsonAsync<ClientView>();
            createdClient.Should().NotBeNull();
            createdClient.Name.Should().Be(createClient.name);
            createdClient.Address.Should().Be(createClient.address);
            createdClient.City.Should().Be(createClient.city);
            createdClient.PostalCode.Should().Be(createClient.postalCode);
            createdClient.CountryId.Should().Be(createClient.countryId);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDataIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { name = "" }), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/clients", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            var createClient = new
            {
                name = "Created Client",
                address = "Created Address",
                city = "Created City",
                postalCode = "Created Postal Code",
                countryId = 2
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(createClient), Encoding.UTF8, "application/json");


            var response = await _httpClient.PostAsync($"/api/Clients", requestContent);
            var createClientResponse = await response.Content.ReadFromJsonAsync<ClientView>();

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Arrange
            int validId = createClientResponse.Id;
            var updatedClient = new
            {
                name = "Updated Client",
                address = "Updated Address",
                city = "Updated City",
                postalCode = "Updated Postal Code",
                countryId = 1
            };
            var updateRequestContent = new StringContent(JsonConvert.SerializeObject(updatedClient), Encoding.UTF8, "application/json");

            // Act
            var updateResponse = await _httpClient.PutAsync($"/api/Clients/{validId}", updateRequestContent);

            // Assert
            updateResponse.EnsureSuccessStatusCode();
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedClientResponse = await updateResponse.Content.ReadFromJsonAsync<ClientView>();

            updatedClientResponse.Should().NotBeNull();
            updatedClientResponse.Id.Should().Be(validId);
            updatedClientResponse.Name.Should().Be("Updated Client");
            updatedClientResponse.Address.Should().Be("Updated Address");
            updatedClientResponse.City.Should().Be("Updated City");
            updatedClientResponse.PostalCode.Should().Be("Updated Postal Code");
            updatedClientResponse.CountryId.Should().Be(1);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { name = "Updated Client" }), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync($"/api/clients/{invalidId}", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var createClient = new
            {
                name = "Created Client",
                address = "Created Address",
                city = "Created City",
                postalCode = "Created Postal Code",
                countryId = 2
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(createClient), Encoding.UTF8, "application/json");


            var response = await _httpClient.PostAsync($"/api/Clients", requestContent);
            var createClientResponse = await response.Content.ReadFromJsonAsync<ClientView>();

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            int validId = createClientResponse.Id;

            // Act
            var deleteResponse = await _httpClient.DeleteAsync($"/api/clients/{validId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            // Act
            var response = await _httpClient.DeleteAsync($"/api/clients/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetByUserId_ReturnsOkWithClients_WhenUserIdIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            int validUserId = 1;
            var requestUri = $"/api/Clients/byUserId/{validUserId}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var clients = await response.Content.ReadAsAsync<List<ClientView>>();

            clients.Should().NotBeNull();
            clients.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetByUserId_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int validUserId = -1;
            var requestUri = $"/api/Clients/byUserId/{validUserId}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
