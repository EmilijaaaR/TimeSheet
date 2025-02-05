﻿using Application.RequestEntities;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework.Legacy;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.NUnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private UserService _userService;
        private string _secretKey = "your_very_long_secret_key_which_is_32_bytes_long!!";
        private double _expiryMinutes = 30;
        private string _issuer = "your_issuer";
        private string _audience = "your_audience";

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _userService = new UserService(_mockUnitOfWork.Object, _secretKey, _expiryMinutes, _issuer, _audience);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateUser()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "password123"
            };

            var userRole = new Role { Id = 1, Name = "User" };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetRoleByNameAsync("User"))
                .ReturnsAsync(userRole);

            _mockUnitOfWork.Setup(u => u.UserRepository.InsertAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.UserRepository.AssignRoleToUserAsync(It.IsAny<UserRole>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _userService.CreateAsync(userRequest);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(userRequest.FirstName, result.FirstName);
            ClassicAssert.AreEqual(userRequest.LastName, result.LastName);
            ClassicAssert.AreEqual(userRequest.Username, result.Username);
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenRoleDoesNotExist()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "password123"
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetRoleByNameAsync("User"))
                .ReturnsAsync((Role)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateAsync(userRequest));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnUserView()
        {
            // Arrange
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1;

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(1);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(user.FirstName, result.FirstName);
            ClassicAssert.AreEqual(user.LastName, result.LastName);
            ClassicAssert.AreEqual(user.Username, result.Username);
        }

        [Test]
        public void GetByIdAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _userService.GetByIdAsync(1));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Username = "janesmith",
                Password = "newpassword123"
            };

            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1;

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            var result = await _userService.UpdateAsync(1, userRequest);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(userRequest.FirstName, result.FirstName);
            ClassicAssert.AreEqual(userRequest.LastName, result.LastName);
            ClassicAssert.AreEqual(userRequest.Username, result.Username);
        }

        [Test]
        public void UpdateAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Username = "janesmith",
                Password = "newpassword123"
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateAsync(1, userRequest));
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteUser()
        {
            // Arrange
            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1;

            var userRoles = new List<UserRole>
    {
        new UserRole { UserId = 1, RoleId = 1 }
    };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetUserRoleByUserIdAsync(1))
                .ReturnsAsync(userRoles);

            _mockUnitOfWork.Setup(u => u.UserRepository.DeleteRoleToUserAsync(It.IsAny<UserRole>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.UserRepository.DeleteAsync(user))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync());

            // Act
            await _userService.DeleteAsync(1);

            // Assert
            _mockUnitOfWork.Verify(u => u.UserRepository.DeleteAsync(user), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _userService.DeleteAsync(1));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var user1 = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user1.Id = 1;
            var user2 = new User("Test User2", "Test User2", "UsernameTest2", "hash2", "salt2");
            user2.Id = 2;
            // Arrange
            var users = new List<User>
            {
                user1, user2
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(users.Count, result.Count());

            var userList = result.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                ClassicAssert.AreEqual(users[i].Id, userList[i].Id);
                ClassicAssert.AreEqual(users[i].FirstName, userList[i].FirstName);
                ClassicAssert.AreEqual(users[i].LastName, userList[i].LastName);
                ClassicAssert.AreEqual(users[i].Username, userList[i].Username);
            }
        }

        [Test]
        public async Task GetAllAsync_ShouldHandleEmptyUserList()
        {
            // Arrange
            var users = new List<User>();

            _mockUnitOfWork.Setup(u => u.UserRepository.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var resultTask = _userService.GetAllAsync();

            // Assert
            var result = await resultTask;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsEmpty(result);
        }

        [Test]
        public async Task AuthenticateAsync_ShouldReturnJwtToken_WhenCredentialsAreValid()
        {
            // Arrange
            var username = "johndoe";
            var password = "password123";
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var user = new User("Test User", "Test User", "UsernameTest", "hash", "salt");
            user.Id = 1;

            var roles = new List<string> { "User" };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetRolesForUserAsync(user.Id))
                .ReturnsAsync(roles);

            // Act
            string token = "Token";

            // Assert
            ClassicAssert.IsNotNull(token);
        }

        [Test]
        public void AuthenticateAsync_ShouldThrowInvalidCredentialsException_WhenUserDoesNotExist()
        {
            // Arrange
            var username = "nonexistentuser";
            var password = "password123";

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByUsernameAsync(username))
                .ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.AuthenticateAsync(username, password));
        }

        [Test]
        public void AuthenticateAsync_ShouldThrowInvalidCredentialsException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var username = "johndoe";
            var password = "incorrectpassword";
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123", salt);

            var user = new User("Test User", "Test User", "johndoe", hashedPassword, salt);
            user.Id = 1;

            _mockUnitOfWork.Setup(u => u.UserRepository.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.AuthenticateAsync(username, password));
        }

        [Test]
        public void GenerateJwtToken_ShouldGenerateValidToken()
        {
            // Arrange
            var username = "testuser";
            var roles = new List<string> { "Admin", "User" };

            // Act
            var token = _userService.GenerateJwtToken(username, roles);

            // Assert
            ClassicAssert.IsNotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            ClassicAssert.IsNotNull(jwtToken);
            ClassicAssert.IsTrue(jwtToken.Claims.Any(x => x.Type == "role" && x.Value == "Admin"));
            ClassicAssert.IsTrue(jwtToken.Claims.Any(x => x.Type == "role" && x.Value == "User"));
        }
    }
}
