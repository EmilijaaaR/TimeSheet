using Application.Exceptions;
using Application.MediatorUserRequest;
using Domain.Repositories;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.MediatorUserHandler
{
    public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserRequest, string?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _secretKey;
        private readonly string _expiryMinutes;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthenticateUserHandler(IUnitOfWork unitOfWork, string secretKey, string expiryMinutes, string issuer, string audience)
        {
            _unitOfWork = unitOfWork;
            _secretKey = secretKey;
            _expiryMinutes = expiryMinutes;
            _issuer = issuer;
            _audience = audience;
        }

        public async Task<string?> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                throw new InvalidCredentialsException();
            }
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new InvalidCredentialsException();
            }

            var roles = await _unitOfWork.UserRepository.GetRolesForUserAsync(user.Id);

            return GenerateJwtToken(request.Username, roles);
        }

        private bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
        {
            var hash = Hash(password, passwordSalt);
            return hash == passwordHash;
        }

        private (string, string) HashPassword(string password)
        {
            string salt = GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return (hash, salt);
        }

        private string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        private string Hash(string password, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        private string GenerateJwtToken(string username, IList<string> roles)
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
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expiryMinutes)),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
