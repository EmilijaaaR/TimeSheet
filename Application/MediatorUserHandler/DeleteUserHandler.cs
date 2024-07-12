using Application.MediatorUserRequest;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MediatorUserHandler
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.");
            }

            var userRoles = await _unitOfWork.UserRepository.GetUserRoleByUserIdAsync(request.Id);
            foreach (var userRole in userRoles)
            {
                await _unitOfWork.UserRepository.DeleteRoleToUserAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.UserRepository.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();

        }
    }

}
