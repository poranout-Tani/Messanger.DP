using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.DTO_s
{
    public class MessageCreateRequest
    {
            [Required]
            public string TextMessage { get; set; } = string.Empty;

            [Required]
            public Guid SenderID { get; set; }

            [Required]
            public Guid ChatID { get; set; }
     
    }
}
