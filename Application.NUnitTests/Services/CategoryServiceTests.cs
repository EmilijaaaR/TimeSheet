using Moq;
using Domain.Entities;
using Domain.Repositories;
using Application.Services;
using NUnit.Framework.Legacy;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private CategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockUnitOfWork.Setup(u => u.CategoryRepository).Returns(_mockCategoryRepository.Object);
            _categoryService = new CategoryService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task CreateAsync_ValidCategory_ReturnsCategoryView()
        {
            // Arrange
            var categoryName = "TestCategory";
            var category = new Category { Name = categoryName };

            _mockCategoryRepository.Setup(repo => repo.InsertAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _categoryService.CreateAsync(categoryName);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(categoryName, result.Name);
            _mockCategoryRepository.Verify(repo => repo.InsertAsync(It.IsAny<Category>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


        [Test]
        public async Task GetByIdAsync_ExistingCategory_ReturnsCategoryView()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "TestCategory" };
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _categoryService.GetByIdAsync(categoryId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(categoryId, result.Id);
            _mockCategoryRepository.Verify(repo => repo.GetByIdAsync(categoryId), Times.Once);
        }

        [Test]
        public void GetByIdAsync_NonExistingCategory_ThrowsArgumentException()
        {
            // Arrange
            var categoryId = 1;
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _categoryService.GetByIdAsync(categoryId));
            ClassicAssert.AreEqual("Category doesn't exist.", ex.Message);
        }

        [Test]
        public async Task UpdateAsync_ExistingCategory_ReturnsUpdatedCategoryView()
        {
            // Arrange
            var categoryId = 1;
            var updatedName = "UpdatedCategory";
            var category = new Category { Id = categoryId, Name = "TestCategory" };
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync(category);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _categoryService.UpdateAsync(categoryId, updatedName);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(updatedName, result.Name);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateAsync_NonExistingCategory_ThrowsArgumentException()
        {
            // Arrange
            var categoryId = 1;
            var updatedName = "UpdatedCategory";
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _categoryService.UpdateAsync(categoryId, updatedName));
            ClassicAssert.AreEqual("Category not found", ex.Message);
        }

        [Test]
        public async Task DeleteAsync_ExistingCategory_DeletesCategory()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "TestCategory" };
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync(category);
            _mockCategoryRepository.Setup(repo => repo.DeleteAsync(category)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            await _categoryService.DeleteAsync(categoryId);

            // Assert
            _mockCategoryRepository.Verify(repo => repo.DeleteAsync(category), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteAsync_NonExistingCategory_ThrowsArgumentException()
        {
            // Arrange
            var categoryId = 1;
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _categoryService.DeleteAsync(categoryId));
            ClassicAssert.AreEqual("Category not found.", ex.Message);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };
            _mockCategoryRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllAsync();

            // Assert
            ClassicAssert.AreEqual(2, result.Count());
            _mockCategoryRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetCategoriesByFiltersAsync_ReturnsFilteredCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };
            _mockCategoryRepository.Setup(repo => repo.GetCategoriesByFiltersAsync(1, 10, null, "Category"))
                .ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCategoriesByFiltersAsync(1, 10, null, "Category");

            // Assert
            ClassicAssert.AreEqual(2, result.Count());
            _mockCategoryRepository.Verify(repo => repo.GetCategoriesByFiltersAsync(1, 10, null, "Category"), Times.Once);
        }
    }
}