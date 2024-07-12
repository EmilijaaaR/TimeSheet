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
    public class GetUserHandler : IRequestHandler<GetUserRequest, UserView>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserView> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.");
            }
            return user.ToView();
        }
    }
}
