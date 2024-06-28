namespace Domain.Entities
{
    public class TimesheetSummary
    {
        public List<TimesheetResult> TimesheetResults { get; set; }
        public decimal TotalHoursAll { get; set; }
    }
}
