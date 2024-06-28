using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task InsertAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task DeleteAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task AssignLeadToProjectAsync(ProjectUser projectUser);
        Task DeleteLeadToProjectAsync(ProjectUser projectUser);
        Task<ProjectUser?> GetByProjectIdAsync(int projectId);
        Task<bool> ExistsAsync(int userId);
        Task<IEnumerable<ProjectUser>> GetByProjectsIdsAsync(IEnumerable<int> projectIds);
        Task AssignRoleToUserAsync(UserRole userRole);
        Task DeleteRoleToUserAsync(UserRole userRole);
        Task<IEnumerable<UserRole>> GetUserRoleByUserIdAsync(int userId);
        Task<List<string>> GetRolesForUserAsync(int userId);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<Role?> GetRoleByIdAsync(int id);
    }
}
