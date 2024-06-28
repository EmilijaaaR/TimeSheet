using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITimesheetRepository
    {
        Task InsertAsync(IEnumerable<Timesheet> timesheets);
        Task DeleteAsync(Timesheet timesheet);
        Task<IEnumerable<Timesheet>> GetTimesheetsAsync(int userId, DateTime startDate, DateTime endDate);
        Task<Timesheet?> GetByIdAsync(int id);
    }

}
