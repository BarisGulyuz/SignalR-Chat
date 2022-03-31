namespace Server.Models
{
    public class PersonalMessages
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string RecieverId { get; set; }
        public string RecieverName { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }
}
