namespace Core.Contracts.DTO_s
{
    public class UserUpdateRequest
    {
        public string Username { get; set; } = string.Empty;

        public string? AvatarURL { get; set; }

        public string? Password { get; set; }
    }
}
