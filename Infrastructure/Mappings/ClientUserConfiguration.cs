using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class ClientUserConfiguration : IEntityTypeConfiguration<ClientUser>
    {
        public void Configure(EntityTypeBuilder<ClientUser> builder)
        {
            builder.HasKey(cu => new { cu.ClientId, cu.UserId });

            builder.HasOne(cu => cu.Client).WithMany(c => c.ClientUsers).HasForeignKey(cu => cu.ClientId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(cu => cu.User).WithMany(u => u.ClientUsers).HasForeignKey(cu => cu.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
