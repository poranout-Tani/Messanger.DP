using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class ChatsConfigurations : IEntityTypeConfiguration<Chats>
    {
        public void Configure(EntityTypeBuilder<Chats> builder)
        {
            builder.HasKey(u => u.ID);

            builder
                .HasOne(c => c.Users)
                .WithMany()
                .HasForeignKey(c => c.UsersID)
                .IsRequired(false);

            builder
                .HasMany(c => c.ChatMembers)
                .WithOne(cm => cm.Chat)
                .HasForeignKey(cm => cm.ChatID);

            builder
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m =>  m.ChatID);
        }
    }
}
