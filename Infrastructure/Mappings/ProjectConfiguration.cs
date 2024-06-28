using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Mappings
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            var converter = new EnumToStringConverter<ProjectStatus>();
            builder
                .Property(p => p.Status)
                .HasConversion(converter)
                .HasDefaultValue(ProjectStatus.Active)
                .IsRequired();

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(25);
            builder.Property(p => p.Description).HasMaxLength(250);

            builder.HasOne(p => p.Client).WithMany(c => c.Projects).HasForeignKey(p => p.ClientId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.HasMany(p => p.Timesheets).WithOne(t => t.Project).HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.HasMany(p => p.Users)
               .WithMany(u => u.Projects)
               .UsingEntity<ProjectUser>(
                   j => j.HasOne(pu => pu.User)
                         .WithMany(u => u.ProjectUsers)
                         .HasForeignKey(pu => pu.UserId),
                   j => j.HasOne(pu => pu.Project)
                         .WithMany(p => p.ProjectUsers)
                         .HasForeignKey(pu => pu.ProjectId),
                   j =>
                   {
                       j.HasKey(pu => new { pu.ProjectId, pu.UserId });
                       j.ToTable("ProjectUser");
                   });
        }
    }

}
