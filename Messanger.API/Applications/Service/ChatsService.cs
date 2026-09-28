using Applications.Interface.Repositories;
using Core.Contracts.Chats;
using Core.Contracts.Messages;
using Core.Contracts.Users;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Service
{
    public class ChatsService
    {
        private readonly IChatsRepository _chatsRepository;

        public ChatsService(IChatsRepository chatsRepository)
        {
            _chatsRepository = chatsRepository;
        }

        public async Task CreateChat(Guid id, Guid interlocutorId, string chatName, Guid userId)
        {
        var Chat = Chats.Create(id, interlocutorId, chatName, userId);

        await _chatsRepository.CreateChat(Chat);
        }

        public async Task<ChatFullInfoResponse?> GetChatFullInfo(Guid chatId)
        {
            var chat = await _chatsRepository.GetFullInfoChats(chatId);

            if (chat == null) return null;

            return new ChatFullInfoResponse(
            chat.ID,
            chat.ChatName,
            chat.Messages.Select(m => new MessagesResponce(
                 m.ID,
                 m.TextMessage,              
                 m.Sender?.Username ?? "System", 
                 m.SenderID,
                 m.Timestamp,
                 false                        
            )).ToList(),
            chat.ChatMembers.Select(cm => new UsersResponse(
                 cm.User.ID,
                 cm.User.Username,
                 null,
                 cm.User.LastSeen,
                 cm.User.Bio,
                 cm.User.Bday
            )).ToList()
            );
        }

        public async Task<List<ChatFullInfoResponse>> GetChatsByUserId(Guid userId)
        {
            var chats = await _chatsRepository.GetUserChats(userId);

            if (chats == null)
            {
                return new List<ChatFullInfoResponse>();
            }

            return chats.Select(chat => new ChatFullInfoResponse(
            chat.ID,
            chat.ChatName,
            chat.Messages.Select(m => new MessagesResponce(
                 m.ID,
                 m.TextMessage,
                 m.Sender?.Username ?? "System",
                 m.SenderID,
                 m.Timestamp,
                 false
            )).ToList(),
            chat.ChatMembers.Select(cm => new UsersResponse(
                 cm.User.ID,
                 cm.User.Username,
                 cm.User.AvatarURL,
                 cm.User.LastSeen,
                 cm.User.Bio,
                 cm.User.Bday
            )).ToList()
            )).ToList();

        }

        public async Task DeleteChat(Guid id)
        {
            await _chatsRepository.DeleteChat(id);
        }


    }
}
