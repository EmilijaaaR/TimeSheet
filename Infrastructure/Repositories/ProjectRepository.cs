using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TimeSheetDbContext _dbContext;

        public ProjectRepository(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(Project project)
        {
            _dbContext.Projects.Add(project);
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _dbContext.Projects
                                   .Include(p => p.Client)
                                   .Include(p => p.Timesheets)
                                   .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task DeleteAsync(Project project)
        {
            _dbContext.Projects.Remove(project);
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _dbContext.Projects
                                   .Include(p => p.Client)
                                   .Include(p => p.Timesheets)
                                   .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByClientIdAsync(int clientId)
        {
            return await _dbContext.Projects
                .Where(p => p.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByIdsAsync(IEnumerable<int> projectIds)
        {
            return await _dbContext.Projects
                                   .Where(p => projectIds.Contains(p.Id))
                                   .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm)
        {
            var query = _dbContext.Projects.AsQueryable();

            if (letter.HasValue)
            {
                string pattern = $"{letter.Value}%";
                query = query.Where(p => EF.Functions.Like(p.Name, pattern));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            return await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
