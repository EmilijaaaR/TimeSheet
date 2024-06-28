using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TimeSheetDbContext _dbContext;

        public CategoryRepository(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(Category category)
        {
            _dbContext.Categories.Add(category);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbContext.Categories
                                   .Include(c => c.Timesheets)
                                   .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task DeleteAsync(Category category)
        {
            _dbContext.Categories.Remove(category);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbContext.Categories
                                   .Include(c => c.Timesheets)
                                   .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm)
        {
            var query = _dbContext.Categories.AsQueryable();

            if (letter.HasValue)
            {
                string pattern = $"{letter.Value}%";
                query = query.Where(c => EF.Functions.Like(c.Name, pattern));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm));
            }

            return await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByIdsAsync(IEnumerable<int> categoryIds)
        {
            return await _dbContext.Categories
                                   .Where(c => categoryIds.Contains(c.Id))
                                   .ToListAsync();
        }
    }
}
