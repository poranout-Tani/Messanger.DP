namespace Core.Contracts.DTO_s
{
    public class UserRegisterRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string AvatarURL { get; set; } = string.Empty;
    }
}
