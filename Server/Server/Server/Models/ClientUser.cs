namespace Server.Models
{
    public class ClientUser
    {
        public string? ConnectionId { get; set; } 
        public string? Name { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
