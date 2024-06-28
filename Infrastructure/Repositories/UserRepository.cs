using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TimeSheetDbContext _dbContext;

        public UserRepository(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(User user)
        {
            _dbContext.Users.Add(user);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                                   .Include(u => u.Timesheets)
                                   .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users
                                   .Include(u => u.Timesheets)
                                   .ToListAsync();
        }

        public async Task AssignLeadToProjectAsync(ProjectUser projectUser)
        {
            _dbContext.ProjectUsers.Add(projectUser);
        }

        public async Task DeleteLeadToProjectAsync(ProjectUser projectUser) 
        {
            _dbContext.ProjectUsers.Remove(projectUser);
        }

        public async Task<bool> ExistsAsync(int userId)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<ProjectUser?> GetByProjectIdAsync(int projectId)
        {
            return await _dbContext.ProjectUsers.FirstOrDefaultAsync(pu => pu.ProjectId == projectId);
        }

        public async Task<IEnumerable<ProjectUser>> GetByProjectsIdsAsync(IEnumerable<int> projectIds)
        {
            return await _dbContext.ProjectUsers
                                   .Where(p => projectIds.Contains(p.ProjectId))
                                   .ToListAsync();
        }

        public async Task AssignRoleToUserAsync(UserRole userRole)
        {
            _dbContext.UserRoles.Add(userRole);
        }

        public async Task DeleteRoleToUserAsync(UserRole userRole)
        {
            _dbContext.UserRoles.Remove(userRole);
        }
        public async Task<IEnumerable<UserRole>> GetUserRoleByUserIdAsync(int userId) 
        {
            return await _dbContext.UserRoles
                                 .Where(ur => ur.UserId == userId)
                                 .ToListAsync();
        }
        public async Task<List<string>> GetRolesForUserAsync(int userId)
        {
            return await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }
        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _dbContext.Roles
                             .SingleOrDefaultAsync(r => r.Name == roleName);
        }
        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _dbContext.Roles
                             .FindAsync(id);
        }
    }

}
