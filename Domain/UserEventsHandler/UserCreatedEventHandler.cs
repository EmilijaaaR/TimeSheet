using Domain.UserEvents;
using MediatR;

namespace Domain.UserEventsHandler
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"User with username {notification.Username} has been created.");
            return Task.CompletedTask;
        }
    }
}
