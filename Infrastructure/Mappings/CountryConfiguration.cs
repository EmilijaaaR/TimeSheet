using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);

            builder.HasMany(c => c.Clients).WithOne(client => client.Country).HasForeignKey(client => client.CountryId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
