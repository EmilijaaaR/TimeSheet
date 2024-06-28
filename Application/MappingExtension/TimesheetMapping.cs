using Application.ViewEntities;
using Domain.Entities;

namespace Application.MappingExtension
{
    public static class TimesheetMapping
    {
        public static TimesheetView ToView(this Timesheet timesheet, int clientId)
        {
            return new TimesheetView
            {
                ClientId = clientId,
                ProjectId = timesheet.ProjectId,
                CategoryId = timesheet.CategoryId,
                Description = timesheet.Description,
                HoursWorked = timesheet.HoursWorked,
                OverTime = timesheet.OverTime
            };
        }

        public static ReportView ToReportView(this Timesheet timesheet)
        {
            return new ReportView
            {
                Date = timesheet.Date,
                UserName = timesheet.User.FirstName + " " + timesheet.User.LastName,
                ProjectName = timesheet.Project.Name,
                CategoryName = timesheet.Category.Name,
                Description = timesheet.Description,
                Time = timesheet.HoursWorked + timesheet.OverTime
            };
        }
    }
}
