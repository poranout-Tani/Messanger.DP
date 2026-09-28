using Applications.Interface.Repositories;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Core.Contracts.Messages;

namespace Applications.Service
{
    public class MessagesServices
    {
        private readonly IMessagesRepository _messagesRepository;

        public MessagesServices(IMessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }

        public async Task AddMessages(Guid chatId, Guid userId, string textmessage)
        {
            var Message = Messages.Create(Guid.NewGuid(), chatId, userId, textmessage);

            await _messagesRepository.AddMessages(Message);
        }
        public async Task<List<MessagesResponce>> GetByChatId(Guid chatId)
        {
            var messages = await _messagesRepository.GetByChat(chatId);

            var response = messages.Select(m => new MessagesResponce(
                m.ID,
                m.TextMessage,
                m.Sender?.Username ?? "System",
                m.SenderID,
                m.Timestamp,
                false
            )).ToList();

            return response;

        }
    }
}
