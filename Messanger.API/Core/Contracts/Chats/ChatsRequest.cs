
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Contracts.Chats
{
    public record ChatsRequest(
    [Required][property: JsonPropertyName("chatName")] string chatName,
    [property: JsonPropertyName("interlocutorId")] Guid? InterlocutorId,
    [property: JsonPropertyName("usersId")] Guid? usersId,
    [property: JsonPropertyName("userIds")] List<Guid>? UserIds
    );
}
