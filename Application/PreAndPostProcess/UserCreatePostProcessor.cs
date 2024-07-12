using Application.MediatorUserRequest;
using Application.ViewEntities;
using MediatR.Pipeline;

namespace Application.PreAndPostProcess
{
    public class UserCreatePostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (request is CreateUserRequest && response is UserView)
            {
                Console.WriteLine("Success: User successfully created.");
            }

            await Task.CompletedTask;
        }
    }
}
