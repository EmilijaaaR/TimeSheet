using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.NUnitTests.Repositories
{
    [TestFixture]
    public class CategoryRepositoryTests
    {
        private TimeSheetDbContext _dbContext;
        private ICategoryRepository _categoryRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            _dbContext = new TimeSheetDbContext(options);
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldInsertCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };

            // Act
            await _categoryRepository.InsertAsync(category);
            await _dbContext.SaveChangesAsync();

            // Assert
            var result = await _dbContext.Categories.FindAsync(category.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(category.Id, result.Id);
            Assert.AreEqual(category.Name, result.Name);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCorrectCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetByIdAsync(category.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(category.Id, result.Id);
            Assert.AreEqual(category.Name, result.Name);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryNotFound()
        {
            // Act
            var result = await _categoryRepository.GetByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            var result1 = await _dbContext.Categories.FindAsync(category.Id);
            Assert.IsNotNull(result1);
            // Act
            await _categoryRepository.DeleteAsync(category);
            await _dbContext.SaveChangesAsync();

            // Assert
            var result = await _dbContext.Categories.FindAsync(category.Id);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" },
                new Category { Id = 3, Name = "Category 3" }
            };
            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(categories.Count, result.Count());
            foreach (var category in categories)
            {
                Assert.IsTrue(result.Any(c => c.Id == category.Id && c.Name == category.Name));
            }
        }

        [Test]
        public async Task GetCategoriesByFiltersAsync_ShouldReturnFilteredCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Apple" },
                new Category { Id = 2, Name = "ABanana" },
                new Category { Id = 3, Name = "Orange" },
                new Category { Id = 4, Name = "Pear" }
            };
            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategoriesByFiltersAsync(pageNumber: 1, pageSize: 2, letter: 'A', searchTerm: "");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(c => c.Name.StartsWith("A")));
        }

        [Test]
        public async Task GetCategoriesByIdsAsync_ShouldReturnCategoriesByIds()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" },
                new Category { Id = 3, Name = "Category 3" }
            };
            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();
            var categoryIds = new List<int> { 1, 3 };

            // Act
            var result = await _categoryRepository.GetCategoriesByIdsAsync(categoryIds);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(c => categoryIds.Contains(c.Id)));
        }
    }
}