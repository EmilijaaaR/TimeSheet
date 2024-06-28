using Domain.Entities;

namespace Domain.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<Timesheet>> GetReportDataAsync(int? userId, int? clientId, int? projectId, int? categoryId, DateTime? startDate, DateTime? endDate);
    }
}
