using Applications.Service;
using Core.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Messanger.API.Endpoints
{
    public static class UsersEndpoints
    {
        public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("register", Register);

            app.MapPost("Login", Login);

            app.MapPut("/profile", Update).DisableAntiforgery();

            app.MapGet("/profile/{id:guid}", GetProfile);

            app.MapPut("/Updatepassword", UpdatePassword).RequireAuthorization();
            return app;
        }

        private static async Task<IResult> Register(RegisterUserRequest request, UsersService usersService)
        {
            await usersService.Register(request.Username, request.Password);
            return Results.Ok();
        }

        private static async Task<IResult> Login(LoginUserRequest request, UsersService usersService)
        {
            try
            {
                var (token, userId) = await usersService.Login(request.Username, request.Password);

                return Results.Ok(new
                {
                    token = token,
                    userid = userId
                });
            }
            catch (Exception ex) when (ex.Message == "Invalid username or password.")
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> Update(Guid id, [FromForm] UpdateProfileRequest request, UsersService usersService, IWebHostEnvironment env)
        {
            try
            {
                var currentUser = await usersService.GetById(id);
                if (currentUser == null)
                {
                    return Results.NotFound(new { error = "Пользователь не найден" });
                }

                string? AvatarUrl = currentUser.AvatarURL;

                if (request.Avatar != null && request.Avatar.Length > 0)
                {
                    var uploadsFolder = Path.Combine(env.WebRootPath, "avatars");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Avatar.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var Stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Avatar.CopyToAsync(Stream);
                    }
                    AvatarUrl = $"/avatars/{uniqueFileName}";
                }

                await usersService.Update(
                    id,
                    request.Username ?? currentUser?.Username ?? "",
                    AvatarUrl,
                    currentUser?.LastSeen ?? DateTime.UtcNow,
                    currentUser.PasswordHash,
                    request.Bio ?? currentUser?.Bio ?? "",
                    request.Bday ?? currentUser?.Bday ?? ""
                );
                var response = new UsersResponse(
                    id,
                    request.Username,
                    AvatarUrl,
                    currentUser.LastSeen,
                    request.Bio,
                    request.Bday
                );

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetProfile(Guid id, UsersService usersService)
{
            var user = await usersService.GetById(id);
            if (user == null) return Results.NotFound();

            return Results.Ok(new {
                username = user.Username,
                avatarUrl = user.AvatarURL,               
                bio      = user.Bio,
                bday     = user.Bday
            });
        }

        private static async Task<IResult> UpdatePassword(UsersService usersService, UsersPasswordUpdate request, HttpContext context)
        {
            var userIdClaim = context.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return Results.BadRequest("Пароль не может быть пустым");
            }

            await usersService.UpdatePassword(userId, request.NewPassword);

            return Results.Ok(new { message = "Пароль успешно обновлен" });
        }

    }
}
