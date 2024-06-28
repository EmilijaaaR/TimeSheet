namespace Application.ViewEntities
{
    public class ProjectView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
    }
}
