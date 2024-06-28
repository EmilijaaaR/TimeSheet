using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(20);

            builder.HasMany(r => r.Users)
                   .WithMany(u => u.Roles)
                   .UsingEntity<UserRole>(
                       j => j.HasOne(ur => ur.User)
                             .WithMany(u => u.UserRoles)
                             .HasForeignKey(ur => ur.UserId),
                       j => j.HasOne(ur => ur.Role)
                             .WithMany(r => r.UserRoles)
                             .HasForeignKey(ur => ur.RoleId),
                       j =>
                       {
                           j.HasKey(ur => new { ur.UserId, ur.RoleId });
                           j.ToTable("UserRole");
                       });
        }
    }
}
