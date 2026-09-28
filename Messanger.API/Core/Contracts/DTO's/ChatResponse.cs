namespace Core.Contracts.DTO_s
{
    public class ChatResponse
    {
        public DateTime TimeCreate { get; set; } = DateTime.UtcNow;
      
        public Guid ID { get; set; }
        
        public string ChatName { get; set; } = string.Empty;
        
        public string? ChatAvatarURL { get; set; }

        public Guid? InterlocutorId { get; set; }

        public MessageResponse? LastMessage { get; set; }
       
    }
}
