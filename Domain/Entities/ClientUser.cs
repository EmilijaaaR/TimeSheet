namespace Domain.Entities
{
    public class ClientUser
    {
        public int ClientId { get; set; }

        public int UserId { get; set; }

        public Client Client { get; set; }

        public User User { get; set; }
    }
}
