namespace Application.RequestEntities
{
    public class ReportRequest
    {
        public int? UserId { get; set; }
        public int? ClientId { get; set; }
        public int? ProjectId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
