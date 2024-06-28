namespace Application.RequestEntities
{
    public class TimesheetUpdateRequest
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CategoryId { get; set; }
        public decimal HoursWorked { get; set; }
        public string Description { get; set; }
        public decimal OverTime { get; set; }
    }
}
