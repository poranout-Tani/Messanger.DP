using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Core.Contracts.Users
{
    public class UpdateProfileRequest
    {
        public string Username { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Bday { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
