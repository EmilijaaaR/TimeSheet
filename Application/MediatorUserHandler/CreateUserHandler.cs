using Application.MappingExtension;
using Application.MediatorUserRequest;
using Application.MediatorUserResponse;
using Application.ViewEntities;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MediatorUserHandler
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, UserView>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IRequestPreProcessor<CreateUserRequest> _preProcessor;
        private readonly IRequestPostProcessor<CreateUserRequest, UserView> _postProcessor;
        public CreateUserHandler(IUnitOfWork unitOfWork, IMediator mediator, IRequestPreProcessor<CreateUserRequest> preProcessor,
        IRequestPostProcessor<CreateUserRequest, UserView> postProcessor)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _preProcessor = preProcessor;
            _postProcessor = postProcessor;
        }

        public async Task<UserView> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            await _preProcessor.Process(request, cancellationToken);
            var (hash, salt) = HashPassword(request.Password);

            var user = new User(request.FirstName, request.LastName, request.Username, hash, salt);

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

            await PublishDomainEvents(user, cancellationToken);

            var response = user.ToView();
            await _postProcessor.Process(request, response, cancellationToken);
            return response;
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

        private async Task PublishDomainEvents(User user, CancellationToken cancellationToken)
        {
            var domainEvents = user.DomainEvents.ToList();
            user.ClearDomainEvents();
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
