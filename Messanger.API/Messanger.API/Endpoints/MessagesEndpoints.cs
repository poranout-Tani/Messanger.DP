using Applications.Service;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Contracts.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Messanger.API.Endpoints
{
    public record MessageRequest(string TextMessage);
    public static class MessagesEndpoints
    {
        public static IEndpointRouteBuilder MapMessagesEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("messages/{chatId:guid}", Send)
                .RequireAuthorization();

            app.MapGet("messages/{chatId:guid}", GetHistory);

            return app;
        }

        private static async Task<IResult> Send(
        Guid ChatID,
        [FromBody] MessageRequest request,
        //Guid SenderID,
        MessagesServices messagesService,
        HttpContext context)
        {
            var userIdClaim = context.User.FindFirst("userId");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Json(new { error = "Клейм 'userId' не найден в токене или не является Guid" }, statusCode: 401);
            }
            //Guid userId = Guid.Parse("a7a73366-7da1-42e1-8e1f-15ebdb030d66");
            await messagesService.AddMessages(ChatID, userId, request.TextMessage);

            return Results.Ok(new { Message = "Отправлено" });

        }

        private static async Task<IResult> GetHistory(Guid chatId, MessagesServices messagesService)
        {
            var messages = await messagesService.GetByChatId(chatId);

            return Results.Ok(messages);
        }
    }
}
