using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(20);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(20);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(23);
            builder.HasIndex(u => u.Username).IsUnique();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.PasswordSalt).IsRequired();

            builder.HasMany(u => u.Timesheets).WithOne(t => t.User).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.HasMany(u => u.Clients).WithMany(c => c.Users).UsingEntity<ClientUser>(j => j.HasOne(cu => cu.Client).WithMany(c => c.ClientUsers)
                         .HasForeignKey(cu => cu.ClientId),j => j.HasOne(cu => cu.User).WithMany(u => u.ClientUsers).HasForeignKey(cu => cu.UserId),j =>
                           {
                               j.HasKey(cu => new { cu.ClientId, cu.UserId });
                               j.ToTable("ClientUser");
                           });

            builder.HasMany(u => u.Projects)
               .WithMany(p => p.Users)
               .UsingEntity<ProjectUser>(
                   j => j.HasOne(pu => pu.Project)
                         .WithMany(p => p.ProjectUsers)
                         .HasForeignKey(pu => pu.ProjectId),
                   j => j.HasOne(pu => pu.User)
                         .WithMany(u => u.ProjectUsers)
                         .HasForeignKey(pu => pu.UserId),
                   j =>
                   {
                       j.HasKey(pu => new { pu.ProjectId, pu.UserId });
                       j.ToTable("ProjectUser");
                   });

            builder.HasMany(u => u.Roles)
       .WithMany(r => r.Users)
       .UsingEntity<UserRole>(
           j => j.HasOne(ur => ur.Role)
                 .WithMany(r => r.UserRoles)
                 .HasForeignKey(ur => ur.RoleId),
           j => j.HasOne(ur => ur.User)
                 .WithMany(u => u.UserRoles)
                 .HasForeignKey(ur => ur.UserId),
           j =>
           {
               j.HasKey(ur => new { ur.UserId, ur.RoleId });
               j.ToTable("UserRole");
           });
        }
    }
}
