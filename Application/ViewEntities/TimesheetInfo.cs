namespace Domain.Entities
{
    public class TimesheetInfo
    {
        public int ProjectId { get; set; }
        public int ClientId { get; set; }
        public int CategoryId { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal OverTime { get; set; }
        public string Description { get; set; }
    }
}
