using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(30);
            builder.Property(c => c.Address).IsRequired().HasMaxLength(50);
            builder.Property(c => c.City).IsRequired().HasMaxLength(25);
            builder.Property(c => c.PostalCode).IsRequired().HasMaxLength(20);
            builder.Property(c => c.CountryId).IsRequired();

            builder.HasOne(c => c.Country).WithMany(country => country.Clients).HasForeignKey(c => c.CountryId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Projects).WithOne(p => p.Client).HasForeignKey(p => p.ClientId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Users).WithMany(u => u.Clients).UsingEntity<ClientUser>(j => j.HasOne(cu => cu.User).WithMany(u => u.ClientUsers)
                    .HasForeignKey(cu => cu.UserId),j => j.HasOne(cu => cu.Client).WithMany(c => c.ClientUsers).HasForeignKey(cu => cu.ClientId),j =>
                    {
                        j.HasKey(cu => new { cu.ClientId, cu.UserId });
                        j.ToTable("ClientUser");
                    });
        }
    }
}
