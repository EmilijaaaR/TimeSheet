using Application.MappingExtension;
using Application.ViewEntities;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services
{
    public class CategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryView> CreateAsync(string name)
        {
            var category = new Category
            {
                Name = name
            };

            await _unitOfWork.CategoryRepository.InsertAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return category.ToView();
        }

        public async Task<CategoryView> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category != null) 
            {
                return category.ToView();
            }
            else 
            {
                throw new ArgumentException("Category doesn't exist.");
            }
        }

        public async Task<CategoryView> UpdateAsync(int id, string name)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Category not found");
            }
            category.Name = name;
            await _unitOfWork.SaveChangesAsync();
            return category.ToView();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Category not found.");
            }
            await _unitOfWork.CategoryRepository.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryView>> GetAllAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return categories.Select(category => category.ToView());
        }

        public async Task<IEnumerable<CategoryView>> GetCategoriesByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm)
        {
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesByFiltersAsync(pageNumber, pageSize, letter, searchTerm);
            return categories.Select(category => category.ToView()).ToList();
        }
    }
}
