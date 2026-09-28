using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DataAccess.Configurations
{
    public class ChatsMembersConfigurations : IEntityTypeConfiguration<ChatMembers>
    {
        public void Configure(EntityTypeBuilder<ChatMembers> builder)
        {
            builder.HasKey(cm => new { cm.ChatID, cm.UserID });

            builder
                .HasOne(cm => cm.Chat)
                .WithMany(cm => cm.ChatMembers)
                .HasForeignKey(cm => cm.ChatID);

            builder
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMembers)
                .HasForeignKey(cm => cm.UserID);

            builder.Property(cm => cm.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Участник");
        }          
    }
}
