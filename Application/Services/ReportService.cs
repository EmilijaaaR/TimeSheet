using Application.MappingExtension;
using Application.RequestEntities;
using Application.ViewEntities;
using Domain.Repositories;

namespace Application.Services
{
    public class ReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReportView>> GenerateReportAsync(ReportRequest request)
        {

            var timesheets = await _unitOfWork.ReportRepository.GetReportDataAsync(
                request.UserId,
                request.ClientId,
                request.ProjectId,
                request.CategoryId,
                request.StartDate,
                request.EndDate
            );

            return timesheets.Select(t => t.ToReportView()).ToList();
        }
    }
}
