using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.Users
{
    public record LoginUserRequest
    (
        [Required] string Username,

        [Required] string Password
    );
}
