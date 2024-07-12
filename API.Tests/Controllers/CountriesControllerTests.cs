using Application.ViewEntities;
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
    public class CountriesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public CountriesControllerTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetAll_ReturnsOkWithCountries_WhenCalledWithValidRole()
        {
            _factory.ResetDatabase();
            // Act
            var response = await _httpClient.GetAsync("/api/countries");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var countries = await response.Content.ReadAsAsync<List<CountryView>>();
            countries.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetById_ReturnsOkWithCountry_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            // Act
            var response = await _httpClient.GetAsync("/api/countries/2");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var country = await response.Content.ReadFromJsonAsync<CountryView>();
            country.Should().NotBeNull();
            country.Id.Should().Be(2);
            country.Name.Should().Be("Country 2");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            // Act
            var response = await _httpClient.GetAsync($"/api/countries/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var name = "New Country";
            var requestUri = $"/api/countries?name={Uri.EscapeDataString(name)}";

            // Act
            var response = await _httpClient.PostAsync(requestUri, null);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var createdCountry = await response.Content.ReadFromJsonAsync<CountryView>();
            createdCountry.Should().NotBeNull();
            createdCountry.Name.Should().Be(name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDataIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { name = "" }), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/countries", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var name = "New Country";
            var requestUri = $"/api/countries?name={Uri.EscapeDataString(name)}";

            var responseCreate = await _httpClient.PostAsync(requestUri, null);

            responseCreate.EnsureSuccessStatusCode();
            responseCreate.StatusCode.Should().Be(HttpStatusCode.OK);

            var createdCountry = await responseCreate.Content.ReadFromJsonAsync<CountryView>();
            int validId = createdCountry.Id;

            // Act
            var response = await _httpClient.PutAsync($"/api/Countries/{validId}?name=Updated Country", null);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedCountryResponse = await response.Content.ReadFromJsonAsync<CountryView>();
            updatedCountryResponse.Should().NotBeNull();
            updatedCountryResponse.Id.Should().Be(validId);
            updatedCountryResponse.Name.Should().Be("Updated Country");
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            // Act
            var response = await _httpClient.PutAsync($"/api/Countries/{invalidId}?name=Updated Country", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            int validId = 1;

            // Act
            var response = await _httpClient.DeleteAsync($"/api/countries/{validId}");

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
            var response = await _httpClient.DeleteAsync($"/api/countries/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

}
