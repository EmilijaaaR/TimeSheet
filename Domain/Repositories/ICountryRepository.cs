using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICountryRepository
    {
        Task InsertAsync(Country country);
        Task<Country?> GetByIdAsync(int id);
        Task DeleteAsync(Country country);
        Task<IEnumerable<Country>> GetAllAsync();
        Task<bool> ExistsAsync(int countryId);
    }
}
