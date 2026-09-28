using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.Users
{
    public record RegisterUserRequest
    (
    [Required] string Username,
    [Required] string Password
    );
    
}
