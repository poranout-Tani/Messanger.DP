namespace Core.Contracts.DTO_s
{
    public class UserResponse
    {
        public Guid ID { get; set; }

        public string Username { get; set; } = string.Empty;

        public string? AvatarURL { get; set; }

        public DateTime LastSeen { get; set; }


    }
}
