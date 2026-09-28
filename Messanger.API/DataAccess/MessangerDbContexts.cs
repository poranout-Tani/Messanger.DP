using DataAccess.Configurations;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class MessangerDbContexts : DbContext
    {                 
            public MessangerDbContexts(DbContextOptions<MessangerDbContexts> options)
                : base(options)
            {
            }            
        
        public DbSet<Users> Users { get; set; }

        public DbSet<Messages> Messages { get; set; }

        public DbSet<ChatMembers> ChatMembers { get; set; }
        
        public DbSet<Chats> Chats { get; set; }

        public DbSet<Contacts> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsersConfigurations());
            modelBuilder.ApplyConfiguration(new ChatsConfigurations());
            modelBuilder.ApplyConfiguration(new MessagesConfigurations());
            modelBuilder.ApplyConfiguration(new ChatsMembersConfigurations());
            modelBuilder.ApplyConfiguration(new ContactsConfigurations());

            base.OnModelCreating(modelBuilder);
        }
    }
}
