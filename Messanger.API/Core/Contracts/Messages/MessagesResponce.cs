using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.Messages
{
    public record MessagesResponce(
    Guid Id,
    string Text,
    string SenderName,
    Guid SenderId,
    DateTime SentAt,
    bool IsEdited
    );
}
