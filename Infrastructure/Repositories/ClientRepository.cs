using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly TimeSheetDbContext _dbContext;

        public ClientRepository(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(Client client)
        {
            _dbContext.Clients.Add(client);
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _dbContext.Clients
                                   .Include(c => c.Country)
                                   .Include(c => c.Projects)
                                   .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task DeleteAsync(Client client)
        {
            _dbContext.Clients.Remove(client);
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _dbContext.Clients
                                   .Include(c => c.Country)
                                   .Include(c => c.Projects)
                                   .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetClientsByUserIdAsync(int userId)
        {
            return await _dbContext.Clients
                .Where(c => c.ClientUsers.Any(cu => cu.UserId == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetClientsByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm)
        {
            var query = _dbContext.Clients.AsQueryable();

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

        public async Task<bool> ExistsAsync(int clientId)
        {
            return await _dbContext.Clients.AnyAsync(c => c.Id == clientId);
        }
    }

}
