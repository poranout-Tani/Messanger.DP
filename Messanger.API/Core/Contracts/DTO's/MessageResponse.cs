namespace Core.Contracts.DTO_s
{
    public class MessageResponse
    {
        public Guid ID { get; set; }

        public string TextMessage { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public bool IsRead { get; set; }

        public Guid SenderID{ get; set; }

        public string SenderName{ get; set;} = string.Empty;
    }
}
