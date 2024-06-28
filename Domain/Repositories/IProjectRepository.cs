using Domain.Entities;

namespace Domain.Repositories
{
    public interface IProjectRepository
    {
        Task InsertAsync(Project project);
        Task<Project> GetByIdAsync(int id);
        Task DeleteAsync(Project project);
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Project>> GetProjectsByClientIdAsync(int clientId);
        Task<IEnumerable<Project>> GetProjectsByIdsAsync(IEnumerable<int> projectIds);
        Task<IEnumerable<Project>> GetProjectsByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm);
    }
}
