using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.Messages
{
    public record MessageRequest(
        [Required] string TextMessage
    );
}
