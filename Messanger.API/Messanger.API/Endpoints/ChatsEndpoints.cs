using Applications.Service;
using Core.Contracts.Chats;
using DataAccess;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Messanger.API.Endpoints
{
    public static class ChatsEndpoints
    {
        public static IEndpointRouteBuilder MapChatsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("Create", CreateChat).RequireAuthorization();

            app.MapGet("Info/{id:guid}", GetChatFullInfo);

            app.MapGet("my-chats", GetUserChats).RequireAuthorization();

            app.MapDelete("Delete", DeleteChat).RequireAuthorization();

            //app.MapGet("TestGetChatsByToken", GetChatsByTokenTest);

            return app;
        }

        private static async Task<IResult> CreateChat(ChatsRequest request, ChatsService chatsService, HttpContext context, MessangerDbContexts dbContext)
        {
            //var chatId = Guid.NewGuid();
            //await chatsService.CreateChat(chatId, request.InterlocutorId, request.chatName, request.usersId);
            //return Results.Ok(new ChatsResponce(chatId, request.chatName));
                var userIdClaim = context.User.FindFirst("userId")?.Value;
                if (!Guid.TryParse(userIdClaim, out var creatorId))
                    return Results.Unauthorized();

                var chatId = Guid.NewGuid();

                if (request.UserIds != null && request.UserIds.Any())
                {
                    await chatsService.CreateChat(chatId, Guid.Empty, request.chatName, creatorId);

                var creatorMember = new Core.Models.ChatMembers
                {
                    ChatID = chatId,
                    UserID = creatorId,
                    JoinedAt = DateTime.UtcNow,
                    Role = "Admin" // Можно задать роль администратора группы
                };
                await dbContext.ChatMembers.AddAsync(creatorMember);

                // Добавляем в цикле всех остальных участников, которых ты выбрал во Flutter
                foreach (var memberId in request.UserIds)
                {
                    if (memberId != creatorId) // Исключаем дублирование создателя
                    {
                        var member = new Core.Models.ChatMembers
                        {
                            ChatID = chatId,
                            UserID = memberId,
                            JoinedAt = DateTime.UtcNow,
                            Role = "Member"
                        };
                        await dbContext.ChatMembers.AddAsync(member);
                    }
                }

                await dbContext.SaveChangesAsync();

                } else  {
                            Guid interlocutor = request.InterlocutorId ?? Guid.Empty;
                            await chatsService.CreateChat(chatId, interlocutor, request.chatName, creatorId);
                            //await chatsService.CreateChat(chatId, request.InterlocutorId ?? Guid.Empty, request.chatName, creatorId);
            }
                return Results.Ok(new ChatsResponce(chatId, request.chatName));
        }

        private static async Task<IResult> GetChatFullInfo(Guid id, ChatsService chatsService)
        {
            var chatInfo = await chatsService.GetChatFullInfo(id);
            if (chatInfo == null)
            {
                return Results.NotFound(new { Message = "Чат не найден" });
            }
            return Results.Ok(chatInfo);
        }

        private static async Task<IResult> GetUserChats(ChatsService chatsService, HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var userChats = await chatsService.GetChatsByUserId(userId);

            return Results.Ok(userChats);
        }

        private static async Task<IResult> DeleteChat(Guid id, ChatsService chatsService)
        {
            await chatsService.DeleteChat(id);
            return Results.Ok();
        }

        //private static async Task<IResult> GetChatsByTokenTest(string token, ChatsService chatsService)
        //{
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return Results.BadRequest(new { Message = "Токен пустой!" });
        //    }

        //    try
        //    {
        //        // Если скопировал вместе со словом "Bearer ", убираем его
        //        var cleanToken = token.StartsWith("Bearer ") ? token.Substring(7).Trim() : token.Trim();

        //        // МАКСИМАЛЬНОЕ УПРОЩЕНИЕ: просто читаем JSON без проверки подписи и времени жизни
        //        var handler = new JwtSecurityTokenHandler();
        //        var jsonToken = handler.ReadJwtToken(cleanToken);

        //        // Ищем userId внутри клеймов токена
        //        var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == "sub")?.Value;

        //        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        //        {
        //            return Results.BadRequest(new { Message = "Не удалось найти userId внутри этого токена" });
        //        }

        //        // Вызываем твой рабочий метод из ChatsService
        //        var userChats = await chatsService.GetChatsByUserId(userId);

        //        // Возвращаем ID пользователя и массив всех его чатов с их ID
        //        return Results.Ok(new
        //        {
        //            UserId = userId,
        //            ChatsId = userChats
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Results.BadRequest(new { Message = "Не удалось прочитать токен. Возможно, строка скопирована не полностью.", Details = ex.Message });
        //    }
        //}

    }

}
