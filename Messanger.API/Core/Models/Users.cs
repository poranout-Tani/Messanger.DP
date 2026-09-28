using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Users
    {
        public Users() { }
        public Users(Guid id, string userName, string passwordHash)
        {
            ID = id;
            Username = userName;
            PasswordHash = passwordHash;
        }

        public Guid ID { get; set; }
     
        public string Username { get; set; } = string.Empty;

        // Храним хеш, а не пароль (для безопасности)
        public string PasswordHash { get; set; } = string.Empty;

        public string? AvatarURL { get; set; }

        public string? Bio { get; set; }

        public string? Bday { get; set; }

        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- НАВИГАЦИОННЫЕ СВОЙСТВА (Связи EFC) ---

        // Связь "Один ко многим": Список всех сообщений, которые отправил этот пользователь
        public List<Messages> Messages { get; set; } = new();

        // Связь "Многие ко многим": Список записей о том, в каких чатах состоит пользователь
        public List<ChatMembers> ChatMembers { get; set; } = new();

        public static Users Create (Guid id, string userName, string  passwordHash)
        {
            return new Users(id, userName, passwordHash);
        }
    }
}
