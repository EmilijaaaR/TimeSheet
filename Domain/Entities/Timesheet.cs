namespace Domain.Entities
{
    public class Timesheet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; }
        public string Description { get; set; }
        public decimal OverTime { get; set; }
        public User User { get; set; }
        public Project Project { get; set; }
        public Category Category { get; set; }
    }
}
