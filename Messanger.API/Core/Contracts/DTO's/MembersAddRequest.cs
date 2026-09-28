using System.ComponentModel.DataAnnotations;

namespace Core.Contracts.DTO_s
{
    public class MembersAddRequest
    {
        [Required]
        public Guid ChatID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        public string Role { get; set; } = "Участник";
    }
}
