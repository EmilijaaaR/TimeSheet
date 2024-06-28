using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly TimeSheetDbContext _dbContext;

        public ReportRepository(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Timesheet>> GetReportDataAsync(int? userId, int? clientId, int? projectId, int? categoryId, DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.Timesheets
                .Include(a => a.User)
                .Include(a => a.Project)
                    .ThenInclude(p => p.Client)
                .Include(a => a.Category)
                .AsQueryable();

            if (userId.HasValue && userId.Value != 0)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            if (clientId.HasValue && clientId.Value != 0)
            {
                query = query.Where(a => a.Project.ClientId == clientId.Value);
            }

            if (projectId.HasValue && projectId.Value != 0)
            {
                query = query.Where(a => a.ProjectId == projectId.Value);
            }

            if (categoryId.HasValue && categoryId.Value != 0)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            if (startDate.HasValue && startDate.Value != null)
            {
                query = query.Where(a => a.Date >= startDate.Value);
            }

            if (endDate.HasValue && endDate.Value != null)
            {
                query = query.Where(a => a.Date <= endDate.Value);
            }

            return await query.ToListAsync();
        }
    }
}
