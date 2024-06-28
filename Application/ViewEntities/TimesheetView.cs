namespace Application.ViewEntities
{
    public class TimesheetView
    {
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal OverTime { get; set; }
    }
}
