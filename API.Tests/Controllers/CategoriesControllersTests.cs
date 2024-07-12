using Application.ViewEntities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace API.Tests.Controllers
{
    public class CategoriesControllersTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public CategoriesControllersTests(CustomWebApplicationFactory<Program> factory)
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
        public async Task GetAll_ReturnsOkWithCategories_WhenCalledWithValidRole()
        {
            _factory.ResetDatabase();
            var response = await _httpClient.GetAsync("/api/categories");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = await response.Content.ReadAsAsync<List<CategoryView>>();
            categories.Should().NotBeNullOrEmpty();
            Assert.NotNull(categories);
            Assert.True(categories.Any());
        }

        [Fact]
        public async Task GetById_ReturnsOkWithCategory_WhenIdIsValid()
        {
            _factory.ResetDatabase();
            // Act
            var response = await _httpClient.GetAsync("/api/Categories/1");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var category = await response.Content.ReadFromJsonAsync<CategoryView>();

            category.Should().NotBeNull();
            category.Id.Should().Be(1);
            category.Name.Should().Be("Test Category 1");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            var requestUri = $"/api/Categories/{invalidId}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCategoriesByFilters_ReturnsOk_WhenFiltersAreValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var query = "?pageNumber=1&pageSize=10&letter=T&searchTerm=Test Category 2";
            var requestUri = $"/api/categories/byFilters{query}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = await response.Content.ReadAsAsync<List<CategoryView>>();
            categories.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetCategoriesByFilters_ReturnsBadRequest_WhenFiltersAreInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var query = "?pageNumber=1&pageSize=10&letter=C&searchTerm=Category";
            var requestUri = $"/api/categories/byFilters{query}";

            // Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var categories = await response.Content.ReadAsAsync<List<CategoryView>>();
            categories.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            var name = "New Category";
            var requestUri = $"/api/categories?name={Uri.EscapeDataString(name)}";

            // Act
            var response = await _httpClient.PostAsync(requestUri, null);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdCategory = await response.Content.ReadFromJsonAsync<CategoryView>();
            createdCategory.Should().NotBeNull();
            createdCategory.Name.Should().Be(name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDataIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { name = "" }), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/categories", requestContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenDataIsValid()
        {
            _factory.ResetDatabase();
            // Arrange
            int validId = 3;

            // Act
            var response = await _httpClient.PutAsync($"/api/Categories/{validId}?name=Updated Category", null);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedCategoryResponse = await response.Content.ReadFromJsonAsync<CategoryView>();

            updatedCategoryResponse.Should().NotBeNull();
            updatedCategoryResponse.Id.Should().Be(validId);
            updatedCategoryResponse.Name.Should().Be("Updated Category");
        }


        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { name = "Updated Category" }), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync($"/api/categories/{invalidId}", requestContent);

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
            var response = await _httpClient.DeleteAsync($"/api/categories/{validId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsInvalid()
        {
            _factory.ResetDatabase();
            // Arrange
            int invalidId = -1;

            // Act
            var response = await _httpClient.DeleteAsync($"/api/categories/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}