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
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersRequest, IEnumerable<UserView>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUsersHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserView>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return users.Select(user => user.ToView());
        }
    }
}
