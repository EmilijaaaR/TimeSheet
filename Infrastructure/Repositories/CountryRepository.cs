using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly TimeSheetDbContext _dbContext;

        public CountryRepository(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(Country country)
        {
            _dbContext.Countries.Add(country);
        }

        public async Task<Country?> GetByIdAsync(int id)
        {
            return await _dbContext.Countries
                                   .Include(c => c.Clients)
                                   .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task DeleteAsync(Country country)
        {
            _dbContext.Countries.Remove(country);
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _dbContext.Countries
                                   .Include(c => c.Clients)
                                   .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int countryId)
        {
            return await _dbContext.Countries.AnyAsync(c => c.Id == countryId);
        }
    }
}
