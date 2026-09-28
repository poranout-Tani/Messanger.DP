using Applications.Service;
using Core.Models;
using DataAccess.Migrations;

namespace Messanger.API.Endpoints
{
    public static class ContactsEndpoints
    {
        public static IEndpointRouteBuilder MapContactsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/contacts/{contactUserId:guid}", AddContact).RequireAuthorization();
            app.MapGet("/contacts", GetContacts).RequireAuthorization();
            app.MapDelete("/contacts/{contactUserId:guid}", DeleteContact).RequireAuthorization();

            return app;
        }

        private static async Task<IResult> AddContact(Guid contactUserId, UsersService usersService, ContactsService contactsService, HttpContext context)
        {
            var ownerId = Guid.Parse(context.User.FindFirst("userId")!.Value);

            if (ownerId == contactUserId)
                return Results.BadRequest(new { error = "Нельзя добавить себя" });

            // проверка что пользователь существует
            var target = await usersService.GetById(contactUserId);
            if (target == null) return Results.NotFound(new { error = "Пользователь не найден" });

            await contactsService.AddContactAsync(ownerId, contactUserId);
            return Results.Ok();
        }

        private static async Task<IResult> GetContacts(ContactsService contactsService, HttpContext context)
        {
            var ownerId = Guid.Parse(context.User.FindFirst("userId")!.Value);
            var contacts = await contactsService.GetMyContactsAsync(ownerId);
            return Results.Ok( contacts.Select(c => new {
                id = c.ContactUserID,
                username = c.ContactUser.Username,
                avatarUrl = c.ContactUser.AvatarURL,
                bio = c.ContactUser.Bio,
            }));
        }

        private static async Task<IResult> DeleteContact(Guid contactUserId, ContactsService contactsService, HttpContext context)
        {
            var ownerId = Guid.Parse(context.User.FindFirst("userId")!.Value);
            await contactsService.RemoveContactAsync(ownerId, contactUserId);
            return Results.Ok();

        }
    }
}
