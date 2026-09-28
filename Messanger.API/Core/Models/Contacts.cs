using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Contacts
    {
        public Contacts() { }
        public Contacts(Guid id, Guid ownerId, Guid contactUserId)
        {
            ID = id;
            OwnerID = ownerId;
            ContactUserID = contactUserId;
            AddedAt = DateTime.UtcNow;
        }

        public Guid ID { get; set; }

        public Guid OwnerID { get; set; }

        public Guid ContactUserID { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Users Owner { get; set; } = null!;

        public Users ContactUser { get; set; } = null!;

        public static Contacts Create(Guid id, Guid ownerId, Guid contactUserId)
        {
            return new Contacts(id, ownerId, contactUserId);
        }
    }
}
