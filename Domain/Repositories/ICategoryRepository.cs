using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICategoryRepository
    {
        Task InsertAsync(Category category);
        Task<Category?> GetByIdAsync(int id);
        Task DeleteAsync(Category category);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetCategoriesByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm);
        Task<IEnumerable<Category>> GetCategoriesByIdsAsync(IEnumerable<int> categoryIds);
    }
}
