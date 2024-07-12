using Application.MappingExtension;
using Application.MediatorUserRequest;
using Application.ViewEntities;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MediatorUserHandler
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, UserView>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserView> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Username = request.Username;

            if (!string.IsNullOrEmpty(request.Password))
            {
                var (hash, salt) = HashPassword(request.Password);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;
            }

            await _unitOfWork.SaveChangesAsync();
            return user.ToView();
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
    }
}
