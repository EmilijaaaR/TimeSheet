using Domain.UserEvents;
using MediatR;

namespace Domain.Entities
{
    public class User
    {
        public User(string firstName, string lastName, string username, string passwordHash, string passwordSalt) 
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;

            AddDomainEvent(new UserCreatedEvent(username));
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<ClientUser> ClientUsers { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        private readonly List<INotification> _domainEvents = new List<INotification>();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
        
    }
}
