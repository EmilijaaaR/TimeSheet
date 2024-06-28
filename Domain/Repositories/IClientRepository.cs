using Domain.Entities;

namespace Domain.Repositories
{
    public interface IClientRepository
    {
        Task InsertAsync(Client client);
        Task<Client?> GetByIdAsync(int id);
        Task DeleteAsync(Client client);
        Task<IEnumerable<Client>> GetAllAsync();
        Task<IEnumerable<Client>> GetClientsByUserIdAsync(int userId);
        Task<IEnumerable<Client>> GetClientsByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm);
        Task<bool> ExistsAsync(int clientId);
    }
}
