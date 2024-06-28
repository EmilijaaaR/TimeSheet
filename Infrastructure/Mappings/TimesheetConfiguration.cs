using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings
{
    public class TimesheetConfiguration : IEntityTypeConfiguration<Timesheet>
    {
        public void Configure(EntityTypeBuilder<Timesheet> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Date).IsRequired();
            builder.Property(t => t.HoursWorked).IsRequired().HasPrecision(5, 2);
            builder.Property(t => t.Description).HasMaxLength(255);
            builder.Property(t => t.OverTime).IsRequired().HasPrecision(5, 2);

            builder.HasOne(t => t.User).WithMany(u => u.Timesheets).HasForeignKey(t => t.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade); 
            builder.HasOne(t => t.Project).WithMany(p => p.Timesheets).HasForeignKey(t => t.ProjectId).IsRequired().OnDelete(DeleteBehavior.Cascade); 
            builder.HasOne(t => t.Category).WithMany(c => c.Timesheets).HasForeignKey(t => t.CategoryId).IsRequired().OnDelete(DeleteBehavior.Cascade); 
        }
    }
 }
