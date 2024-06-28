using API.RequestEntities;
using API.ViewEntities;
using Application.Enums;
using Application.MappingExtension;
using Application.RequestEntities;
using Application.ViewEntities;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services
{
    public class TimesheetService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TimesheetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TimesheetView>> CreateAsync(int userId, DateTime date, IEnumerable<TimesheetRequest> timesheetRequests)
        {
            bool userIdCheck = await _unitOfWork.UserRepository.ExistsAsync(userId);
            if (!userIdCheck)
            {
                throw new ArgumentException("User does not exist.");
            }
            var timesheets = new List<Timesheet>();
            var projectsIds = timesheetRequests.Select(tr => tr.ProjectId).Distinct().ToList();
            var projects = await _unitOfWork.ProjectRepository.GetProjectsByIdsAsync(projectsIds);
            var categoriesIds = timesheetRequests.Select(tr => tr.CategoryId).Distinct().ToList();
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesByIdsAsync(categoriesIds);

            int clientId = 0;

            foreach (var timesheetRequest in timesheetRequests)
            {
                var project = projects.FirstOrDefault(p => p.Id == timesheetRequest.ProjectId);
                if (project != null)
                {
                    var category = categories.FirstOrDefault(c => c.Id == timesheetRequest.CategoryId);
                    if (category == null) 
                    {
                        throw new ArgumentException("Category does not exist.");
                    }
                    clientId = project.ClientId;

                    var timesheet = new Timesheet
                    {
                        UserId = userId,
                        ProjectId = timesheetRequest.ProjectId,
                        CategoryId = timesheetRequest.CategoryId,
                        Date = date,
                        HoursWorked = timesheetRequest.HoursWorked,
                        Description = timesheetRequest.Description,
                        OverTime = timesheetRequest.OverTime
                    };
                    timesheets.Add(timesheet);
                }
                else 
                {
                    throw new ArgumentException("Project does not exist.");
                }
            }

            await _unitOfWork.TimesheetRepository.InsertAsync(timesheets);
            await _unitOfWork.SaveChangesAsync();
            return timesheets.Select(t => t.ToView(clientId)).ToList();
        }

        public async Task DeleteAsync(int id)
        {
            var timesheet = await _unitOfWork.TimesheetRepository.GetByIdAsync(id);
            if (timesheet == null)
            {
                throw new ArgumentException("Timesheet does not exist.");
            }
            await _unitOfWork.TimesheetRepository.DeleteAsync(timesheet);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TimesheetSummary> GetTimesheetsWithStatusAsync(int userId, DateTime startDate, DateTime endDate)
        {
            bool userIdCheck = await _unitOfWork.UserRepository.ExistsAsync(userId);
            if (!userIdCheck)
            {
                throw new ArgumentException("User does not exist.");
            }
            var timesheets = await _unitOfWork.TimesheetRepository.GetTimesheetsAsync(userId, startDate, endDate);

            var groupedTimesheets = timesheets
                .GroupBy(t => t.Date)
                .Select(group => new TimesheetResult
                {
                    Date = group.Key,
                    Timesheets = group.Select(t => new TimesheetInfo
                    {
                        ProjectId = t.ProjectId,
                        ClientId = t.Project.ClientId,
                        CategoryId = t.CategoryId,
                        HoursWorked = t.HoursWorked,
                        OverTime = t.OverTime,
                        Description = t.Description
                    }).ToList(),
                    TotalHours = group.Sum(t => t.HoursWorked + t.OverTime),
                    Status = CalculateStatus(group.Sum(t => t.HoursWorked + t.OverTime)).ToString()
                }).ToList();

            var totalHoursAll = groupedTimesheets.Sum(t => t.TotalHours);

            return new TimesheetSummary
            {
                TimesheetResults = groupedTimesheets,
                TotalHoursAll = totalHoursAll
            };
        }

        public async Task<IEnumerable<TimesheetView>> UpdateTimesheetsWithStatusAsync(int userId, DateTime startDate, DateTime endDate, IEnumerable<TimesheetUpdateRequest> timesheetRequests)
        {
            bool userIdCheck = await _unitOfWork.UserRepository.ExistsAsync(userId);
            if (!userIdCheck) 
            {
                throw new ArgumentException("User does not exist.");
            }
            var existingTimesheets = await _unitOfWork.TimesheetRepository.GetTimesheetsAsync(userId, startDate, endDate);
            var categoryIds = timesheetRequests.Select(tr => tr.CategoryId).Distinct().ToList();
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesByIdsAsync(categoryIds);

            var projectIds = timesheetRequests.Select(tr => tr.ProjectId).Distinct().ToList();
            var projects = await _unitOfWork.ProjectRepository.GetProjectsByIdsAsync(projectIds);

            int clientId = 0;

            foreach (var timesheetRequest in timesheetRequests)
            {
                var project = projects.FirstOrDefault(p => p.Id == timesheetRequest.ProjectId);
                var category = categories.FirstOrDefault(c => c.Id == timesheetRequest.CategoryId);
                if (project != null)
                { 
                    clientId = project.ClientId;
                    if (clientId == 0)
                        throw new ArgumentException("Client does not exist.");
                    if (category == null)
                    {
                        throw new ArgumentException("Category does not exist.");
                    }
                        var existingTimesheet = existingTimesheets.FirstOrDefault(t => t.Id == timesheetRequest.Id);

                    if (existingTimesheet != null)
                    {
                        existingTimesheet.ProjectId = timesheetRequest.ProjectId;
                        existingTimesheet.CategoryId = timesheetRequest.CategoryId;
                        existingTimesheet.HoursWorked = timesheetRequest.HoursWorked;
                        existingTimesheet.OverTime = timesheetRequest.OverTime;
                        existingTimesheet.Description = timesheetRequest.Description;
                    }
                    else 
                    {
                        throw new ArgumentException("Timesheet does not exist.");
                    }
                }
                else 
                {
                    throw new ArgumentException("Project does not exist.");
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return existingTimesheets.Select(t => t.ToView(clientId)).ToList();
        }

        private WorkStatus CalculateStatus(decimal totalHours)
        {
            if (totalHours >= 7.5m)
            {
                return WorkStatus.Sufficient;
            }
            else if (totalHours > 0 && totalHours<7.5m)
            {
                return WorkStatus.Insufficient;
            }
            else
            {
                return WorkStatus.NotWorked;
            }
        }
    }

}
