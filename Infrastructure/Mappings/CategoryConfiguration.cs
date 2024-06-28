using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(255);

            builder.HasMany(c => c.Timesheets).WithOne(t => t.Category).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Cascade).IsRequired();
        }
    }
}
