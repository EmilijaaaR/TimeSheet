namespace Domain.Entities
{
    public class User
    {
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
    }
}
