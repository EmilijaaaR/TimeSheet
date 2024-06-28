namespace Application.RequestEntities
{
    public class ProjectUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
    }
}
