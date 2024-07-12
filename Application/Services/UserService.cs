using Application.RequestEntities;
using Domain.Entities;
using Domain.Repositories;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Application.ViewEntities;
using Application.MappingExtension;
using Application.Exceptions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _secretKey;
        private readonly double _expiryMinutes;
        private readonly string _issuer;
        private readonly string _audience;

        public UserService(IUnitOfWork unitOfWork, string secretKey, double expiryMinutes, string issuer, string audience)
        {
            _unitOfWork = unitOfWork;
            _secretKey = secretKey;
            _expiryMinutes = expiryMinutes;
            _issuer = issuer;
            _audience = audience;
        }

        public async Task<UserView> CreateAsync(UserRequest userRequest)
        {
            var (hash, salt) = HashPassword(userRequest.Password);
            var user = new User(userRequest.FirstName, userRequest.LastName, userRequest.Username, hash, salt);
            await _unitOfWork.UserRepository.InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userRole = await _unitOfWork.UserRepository.GetRoleByNameAsync("User");
            if (userRole == null)
            {
                throw new ArgumentException("Role does not exist.");
            }
                var userRoleEntity = new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id
                };
            await _unitOfWork.UserRepository.AssignRoleToUserAsync(userRoleEntity);
            await _unitOfWork.SaveChangesAsync();
            return user.ToView();
        }

        public async Task<UserView> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.");
            }
            return user.ToView();
        }

        public async Task<UserView> UpdateAsync(int id, UserRequest userRequest)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.");
            }

            user.FirstName = userRequest.FirstName;
            user.LastName = userRequest.LastName;
            user.Username = userRequest.Username;

            if (!string.IsNullOrEmpty(userRequest.Password))
            {
                var (hash, salt) = HashPassword(userRequest.Password);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;
            }

            await _unitOfWork.SaveChangesAsync();
            return user.ToView();
        }


        public async Task DeleteAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.");
            }

            var userRoles = await _unitOfWork.UserRepository.GetUserRoleByUserIdAsync(id);
            foreach (var userRole in userRoles)
            {
                await _unitOfWork.UserRepository.DeleteRoleToUserAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.UserRepository.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserView>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return users.Select(user => user.ToView());
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var user = await _unitOfWork.UserRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidCredentialsException();
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new InvalidCredentialsException();
            }

            var roles = await _unitOfWork.UserRepository.GetRolesForUserAsync(user.Id);

            return GenerateJwtToken(username, roles);
        }

        public string GenerateJwtToken(string username, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var claims = new List<Claim>
    {
            new Claim(ClaimTypes.Name, username)
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
        {
            var hash = Hash(password, passwordSalt);
            return hash == passwordHash;
        }

        private (string, string) HashPassword(string lozinka)
        {
            string salt = GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(lozinka, salt);
            return (hash, salt);
        }

        private string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        private string Hash(string lozinka, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(lozinka, salt);
        }

    }
}
