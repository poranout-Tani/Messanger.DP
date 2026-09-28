using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Messages
    {
        public Messages() { }
        public Messages(Guid id, Guid senderId, Guid chatId,string textmessage)
        {
            ID = id;
            ChatID = chatId;
            SenderID = senderId;
            TextMessage = textmessage;
            Timestamp = DateTime.UtcNow;
        }
        public Guid ID { get; set; }

        public string TextMessage { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        // Внешние ключи
        public Guid SenderID { get; set; }
        public Guid ChatID { get; set; }

        // --- Навигационные свойства ---
        // Эти свойства позволяют написать: message.Sender.Username
        public virtual Users Sender { get; set; } = null!;
        public virtual Chats Chat { get; set; } = null!;

        public static Messages Create (Guid id, Guid chatId, Guid senderId, string textmessage)
        {
            return new Messages(id, senderId, chatId, textmessage);
        }
    }
}
