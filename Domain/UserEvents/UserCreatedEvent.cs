using MediatR;

namespace Domain.UserEvents
{
    public class UserCreatedEvent : INotification
    {
        public string Username { get; }

        public UserCreatedEvent(string username)
        {
            Username = username;
        }
    }
}
