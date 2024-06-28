using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly TimeSheetDbContext _dbContext;

        public TimesheetRepository(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(IEnumerable<Timesheet> timesheets)
        {
            _dbContext.Timesheets.AddRange(timesheets);
        }

        public async Task DeleteAsync(Timesheet timesheet)
        {
            _dbContext.Timesheets.Remove(timesheet);
        }

        public async Task<IEnumerable<Timesheet>> GetTimesheetsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Timesheets
                .Include(t => t.Project)
                    .ThenInclude(p => p.Client)
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();
        }
        public async Task<Timesheet?> GetByIdAsync(int id) 
        {
            return await _dbContext.Timesheets
                                       .Include(t => t.Project)
                                    .ThenInclude(p => p.Client)
                .Include(t => t.Category)
                                       .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
