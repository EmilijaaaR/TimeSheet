using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class ProjectUserConfiguration : IEntityTypeConfiguration<ProjectUser>
    {
        public void Configure(EntityTypeBuilder<ProjectUser> builder)
        {
            builder.HasKey(pu => new { pu.ProjectId, pu.UserId });

            builder.HasOne(pu => pu.Project).WithMany(p => p.ProjectUsers).HasForeignKey(pu => pu.ProjectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pu => pu.User).WithMany(u => u.ProjectUsers).HasForeignKey(pu => pu.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }

}
