using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.NUnitTests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private TimeSheetDbContext _dbContext;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TimeSheetDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TimeSheetDbContext(options);
            _userRepository = new UserRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldInsertUser()
        {
            // Arrange
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1;
            // Act
            await _userRepository.InsertAsync(user);
            await _dbContext.SaveChangesAsync();

            //Assert
            Assert.AreEqual(1, _dbContext.Users.Count());
            Assert.IsTrue(_dbContext.Users.Contains(user));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1; 
            var user2 = new User("Test User2", "Test User2", "UsernameTest2", "hash2", "salt2");
            user2.Id = 2;

            await _dbContext.Users.AddRangeAsync(user,user2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByIdAsync(user.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
        }

        [Test]
        public async Task GetByUsernameAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var user = new User ("Test User", "Test User", "UsernameTest", "hash", "salt" );
            user.Id = 1;
            var user2 = new User ("Test User2", "Test User2", "UsernameTest2", "hash2", "salt2" );
            user2.Id = 2;
            var username = "UsernameTest";
            await _dbContext.Users.AddRangeAsync(user, user2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByUsernameAsync(username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Username, result.Username);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            // Arrange
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1; 
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.AreEqual(0, _dbContext.Users.Count());
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var user1 = new User("Test User1", "Test User1", "UsernameTest1", "hash1", "salt1");
            user1.Id = 1;
            var user2 = new User("Test User2", "Test User2", "UsernameTest2", "hash2", "salt2");
            user1.Id = 2;
            await _dbContext.Users.AddRangeAsync(user1, user2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.Contains(user1, result.ToList());
            Assert.Contains(user2, result.ToList());
        }

        [Test]
        public async Task AssignLeadToProjectAsync_ShouldAssignLead()
        {
            // Arrange
            var projectUser = new ProjectUser { ProjectId = 1, UserId = 1 };

            // Act
            await _userRepository.AssignLeadToProjectAsync(projectUser);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.IsTrue(_dbContext.ProjectUsers.Contains(projectUser));
        }

        [Test]
        public async Task DeleteLeadToProjectAsync_ShouldRemoveLead()
        {
            // Arrange
            var projectUser = new ProjectUser { ProjectId = 1, UserId = 1 };
            await _dbContext.ProjectUsers.AddAsync(projectUser);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteLeadToProjectAsync(projectUser);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.IsFalse(_dbContext.ProjectUsers.Contains(projectUser));
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueIfExists()
        {
            // Arrange
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1;
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.ExistsAsync(user.Id);
            
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseIfNotExists()
        {
            // Act
            var result = await _userRepository.ExistsAsync(1);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetByProjectIdAsync_ShouldReturnCorrectProjectUser()
        {
            // Arrange
            var projectUser = new ProjectUser { ProjectId = 1, UserId = 1 };
            await _dbContext.ProjectUsers.AddAsync(projectUser);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByProjectIdAsync(projectUser.ProjectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projectUser.ProjectId, result.ProjectId);
        }

        [Test]
        public async Task GetByProjectsIdsAsync_ShouldReturnCorrectProjectUsers()
        {
            // Arrange
            var projectUser1 = new ProjectUser { ProjectId = 1, UserId = 1 };
            var projectUser2 = new ProjectUser { ProjectId = 2, UserId = 2 };
            await _dbContext.ProjectUsers.AddRangeAsync(projectUser1, projectUser2);
            await _dbContext.SaveChangesAsync();

            var projectIds = new List<int> { 1, 2 };

            // Act
            var result = await _userRepository.GetByProjectsIdsAsync(projectIds);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.Contains(projectUser1, result.ToList());
            Assert.Contains(projectUser2, result.ToList());
        }

        [Test]
        public async Task AssignRoleToUserAsync_ShouldAssignRole()
        {
            // Arrange
            var userRole = new UserRole { UserId = 1, RoleId = 1 };

            // Act
            await _userRepository.AssignRoleToUserAsync(userRole);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.IsTrue(_dbContext.UserRoles.Contains(userRole));
        }

        [Test]
        public async Task DeleteRoleToUserAsync_ShouldRemoveRole()
        {
            // Arrange
            var userRole = new UserRole { UserId = 1, RoleId = 1 };
            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteRoleToUserAsync(userRole);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.IsFalse(_dbContext.UserRoles.Contains(userRole));
        }

        [Test]
        public async Task GetUserRoleByUserIdAsync_ShouldReturnCorrectUserRoles()
        {
            // Arrange
            var userRole1 = new UserRole { UserId = 1, RoleId = 1 };
            var userRole2 = new UserRole { UserId = 1, RoleId = 2 };
            await _dbContext.UserRoles.AddRangeAsync(userRole1, userRole2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserRoleByUserIdAsync(userRole1.UserId);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.Contains(userRole1, result.ToList());
            Assert.Contains(userRole2, result.ToList());
        }

        [Test]
        public async Task GetRolesForUserAsync_ShouldReturnCorrectRoles()
        {
            // Arrange
            var role1 = new Role { Id = 1, Name = "Admin" };
            var role2 = new Role { Id = 2, Name = "User" };
            var userRole1 = new UserRole { UserId = 1, RoleId = 1, Role = role1 };
            var userRole2 = new UserRole { UserId = 1, RoleId = 2, Role = role2 };

            await _dbContext.Roles.AddRangeAsync(role1, role2);
            await _dbContext.UserRoles.AddRangeAsync(userRole1, userRole2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetRolesForUserAsync(1);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.Contains("Admin", result);
            Assert.Contains("User", result);
        }

        [Test]
        public async Task GetRoleByNameAsync_ShouldReturnCorrectRole()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetRoleByNameAsync("Admin");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(role.Id, result.Id);
        }

        [Test]
        public async Task GetRoleByIdAsync_ShouldReturnCorrectRole()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetRoleByIdAsync(role.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(role.Id, result.Id);
        }

    }
}
