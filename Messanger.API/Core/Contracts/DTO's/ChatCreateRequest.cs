using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.DTO_s;

public class ChatCreateRequest
{
    public string ChatName { get; set; } = string.Empty;

    public string? ChatAvatarURL { get; set; }

    public List<Guid> ParticipantIds { get; set; } = new List<Guid>();
    public Guid InterlocutorId { get; set; }
}
