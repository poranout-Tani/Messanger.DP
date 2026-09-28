using Applications.Service;

namespace Messanger.API.Endpoints
{
    public static class ChatMembersEndpoints
    {
        public static IEndpointRouteBuilder MapChatsMembersEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/chat-members");

            group.MapGet("{chatId:guid}", GetMembers);
            group.MapPost("add", AddMember);
            group.MapDelete("remove", RemoveMember);

            return app;
        }

        private static async Task<IResult> GetMembers(Guid chatId, ChatMembersServices membersService)
        {
            var members = await membersService.GetChatMembersAsync(chatId);
            return Results.Ok(members);
        }

        private static async Task<IResult> AddMember(Guid chatId, Guid userId, ChatMembersServices membersService)
        {
            await membersService.AddMemberAsync(chatId, userId);
            return Results.Ok(new { Message = "Пользователь успешно добавлен в чат" });
        }

        private static async Task<IResult> RemoveMember(Guid chatId, Guid userId, ChatMembersServices membersService)
        {
            await membersService.DeleteMemberAsync(chatId, userId);
            return Results.Ok(new { Message = "Пользователь удален из чата" });
        }

    }
}
