
using Core.Contracts.Messages;

namespace Core.Contracts.DTO_s
{
    public class ChatDetailsResponse
    {
        public Guid ID { get; set; }

        public string ChatName { get; set; } = string.Empty;

        public List<MessagesResponce> Messages { get; set; } = new();

        public List<UserResponse> Members { get; set; } = new();
    }
}
