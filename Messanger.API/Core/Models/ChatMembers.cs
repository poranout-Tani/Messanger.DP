using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ChatMembers
    {
        // Внешние ключи
        public Guid ChatID { get; set; }
        public Guid UserID { get; set; }

        // Дополнительные данные о членстве
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "Member"; // Например: Admin, Member

        // --- Навигационные свойства ---
        public Chats Chat { get; set; } = null!;
        public Users User { get; set; } = null!;
    }
}
