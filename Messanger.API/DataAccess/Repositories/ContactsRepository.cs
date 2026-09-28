using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Applications.Interface.Repositories;

namespace DataAccess.Repositories
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly MessangerDbContexts _dbContext;

        public ContactsRepository(MessangerDbContexts dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Guid ownerId, Guid contactUserId)
        {
            // проверка что такой контакт уже не существует
            var exists = await _dbContext.Contacts
                .AnyAsync(ct => ct.OwnerID == ownerId && ct.ContactUserID == contactUserId);
            if (exists) return;

            await _dbContext.Contacts.AddAsync(new Contacts
            {
                ID = Guid.NewGuid(),
                OwnerID = ownerId,
                ContactUserID = contactUserId,
            });

            await _dbContext.SaveChangesAsync();

        }

        public async Task<List<Contacts>> GetByOwner(Guid ownerId)
        {
            return await _dbContext.Contacts
                .AsNoTracking()
                .Include(c => c.ContactUser)
                .Where(c => c.OwnerID == ownerId)
                .ToListAsync();
        }

        public async Task Delete(Guid ownerId, Guid contactUserId)
        {
            await _dbContext.Contacts
                .Where(ct => ct.OwnerID == ownerId && ct    .ContactUserID == contactUserId)
                .ExecuteDeleteAsync();
        }

    }
}
