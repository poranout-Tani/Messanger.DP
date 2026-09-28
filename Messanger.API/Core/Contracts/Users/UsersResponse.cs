namespace Core.Contracts.Users;

    public record UsersResponse
    (
        Guid ID, 
        string Username,
        string? AvatarURL,
        DateTime LastSeen,
        string? Bday,
        string? Bio
    );