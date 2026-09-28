using Applications.Interface.Repositories;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Service
{
    public class ContactsService
    {
        private readonly IContactsRepository _contactsRepository;

        public ContactsService(IContactsRepository contactsRepository)
        {
            _contactsRepository = contactsRepository;
        }

        public async Task AddContactAsync(Guid ownerId, Guid contactUserId)
        {
            // Здесь можно добавить бизнес-логику, например: 
            // нельзя добавить самого себя в контакты
            if (ownerId == contactUserId)
                throw new Exception("Нельзя добавить себя в контакты.");

            await _contactsRepository.Add(ownerId, contactUserId);
        }

        public async Task<List<Contacts>> GetMyContactsAsync(Guid ownerId)
        {
            return await _contactsRepository.GetByOwner(ownerId);
        }

        public async Task RemoveContactAsync(Guid ownerId, Guid contactUserId)
        {
            await _contactsRepository.Delete(ownerId, contactUserId);
        }

    }
}
