namespace Domain.Entities
{
    public class TimesheetResult
    {
        public DateTime Date { get; set; }
        public List<TimesheetInfo> Timesheets { get; set; }
        public decimal TotalHours { get; set; }
        public string Status { get; set; }
    }
}
