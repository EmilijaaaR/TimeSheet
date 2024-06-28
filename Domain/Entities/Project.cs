using Domain.Enums;

namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public ProjectStatus Status { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }
    }
}
