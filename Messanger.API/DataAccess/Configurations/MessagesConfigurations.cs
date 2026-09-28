using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class MessagesConfigurations : IEntityTypeConfiguration<Messages>
    {
        public void Configure(EntityTypeBuilder<Messages> builder)
        {
            builder.HasKey(u => u.ID);

            //builder.ToTable("Messages");
            //builder.Property(m => m.ChatID).HasColumnName("ChatID");
            //builder.Property(m => m.SenderID).HasColumnName("SenderID");
            //builder.Property(m => m.TextMessage).HasColumnName("textmessage");
            //builder.Property(m => m.Timestamp).HasColumnName("datesend");

            builder
                .HasOne(m => m.Chat)
                .WithMany(m => m.Messages)
                .HasForeignKey(m => m.ChatID);

            builder
                .HasOne(m => m.Sender)
                .WithMany(m => m.Messages)
                .HasForeignKey(m => m.SenderID);
        }
    }
}
