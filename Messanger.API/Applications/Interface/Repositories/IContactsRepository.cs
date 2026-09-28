using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Interface.Repositories
{
    public interface IContactsRepository
    {
        Task Add(Guid ownerId, Guid contactUserId);

        Task Delete(Guid ownerId, Guid contactUserId);

        Task<List<Contacts>> GetByOwner(Guid ownerId);
    }
}
