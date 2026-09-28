using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Chats
    {
        public Chats() { }
        public Chats(Guid id,Guid interlocutorid, string chatName, Guid usersId)
        {
            ID = id;
            InterlocutorId = interlocutorid;
            ChatName = chatName;
            UsersID = usersId;

        }

        public Guid ID { get; set; }

        public string ChatName { get; set; } = string.Empty;

        public DateTime TimeCreate { get; set; } = DateTime.UtcNow;

        // Поле для личных сообщений (ID собеседника)
        public Guid InterlocutorId { get; set; }

        // --- Навигационные свойства ---
        // Список участников (через таблицу связки)
        public List<ChatMembers> ChatMembers { get; set; } = new();

        // Все сообщения в этом чате
        public List<Messages> Messages { get; set; } = new();

        public Guid? UsersID {  get; set; }

        public Users? Users { get; set; }

        public static Chats Create(Guid id, Guid interlocutorid, string chatName, Guid usersId)
        {
            return new Chats(id, interlocutorid, chatName, usersId);
        }
    }
}
