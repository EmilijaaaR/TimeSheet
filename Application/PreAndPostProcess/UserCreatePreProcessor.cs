using Application.MediatorUserRequest;
using MediatR.Pipeline;

namespace Application.PreAndPostProcess
{
    public class UserCreatePreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if (request is CreateUserRequest)
            {
                Console.WriteLine("Announcement: User creation started.");
            }

            await Task.CompletedTask;
        }
    }
}
