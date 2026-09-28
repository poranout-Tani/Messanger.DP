using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class ContactsConfigurations : IEntityTypeConfiguration<Contacts>
    {
        public void Configure(EntityTypeBuilder<Contacts> builder)
        {
            builder.HasKey(ct => ct.ID);

            builder.HasOne(ct => ct.Owner)
                    .WithMany()
                    .HasForeignKey(ct => ct.OwnerID)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ct => ct.ContactUser)
                   .WithMany()
                   .HasForeignKey(ct => ct.ContactUserID)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
}