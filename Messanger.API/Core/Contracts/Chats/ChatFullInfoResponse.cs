using Core.Contracts.DTO_s;
using Core.Contracts.Messages;
using Core.Contracts.Users;

namespace Core.Contracts.Chats
{
    public record ChatFullInfoResponse(
    Guid Id,
    string ChatName,
    List<MessagesResponce> Messages,
    List<UsersResponse> Members
    );
}
