using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Mappings;

namespace Infrastructure.Data
{
    public class TimeSheetDbContext : DbContext
    {
        public TimeSheetDbContext(DbContextOptions<TimeSheetDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ClientUser> ClientUsers { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TimesheetConfiguration());
            modelBuilder.ApplyConfiguration(new ClientUserConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectUserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        }
    }
}
