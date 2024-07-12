using Application.ViewEntities;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using FluentAssertions;
using API;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace API.NUnitTests.Controllers
{
    [TestFixture]
    public class CategoriesControllerTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestApplicationFactory _factory;

        public CategoriesControllerTests()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryView>>(content);
            categories.Should().NotBeNull();
            categories.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task GetById_ShouldReturnCategory()
        {
            // Arrange
            int testId = 1; // Postavite na ID koji postoji u bazi

            // Act
            var response = await _client.GetAsync($"/api/categories/{testId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<CategoryView>(content);
            category.Should().NotBeNull();
            category.Id.Should().Be(testId);
        }

        [Test]
        public async Task Create_ShouldCreateNewCategory()
        {
            // Arrange
            var newCategory = new { name = "New Category" };
            var content = new StringContent(JsonConvert.SerializeObject(newCategory), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/categories", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created); // Created
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<CategoryView>(responseContent);
            createdCategory.Should().NotBeNull();
            createdCategory.Name.Should().Be("New Category");
        }

        [Test]
        public async Task Update_ShouldUpdateCategory()
        {
            // Arrange
            int testId = 1; // Postavite na ID koji postoji u bazi
            var updatedCategory = new { name = "Updated Category" };
            var content = new StringContent(JsonConvert.SerializeObject(updatedCategory), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/categories/{testId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<CategoryView>(responseContent);
            category.Should().NotBeNull();
            category.Name.Should().Be("Updated Category");
        }

        [Test]
        public async Task Delete_ShouldDeleteCategory()
        {
            // Arrange
            int testId = 1; // Postavite na ID koji postoji u bazi

            // Act
            var response = await _client.DeleteAsync($"/api/categories/{testId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent); // No Content
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
