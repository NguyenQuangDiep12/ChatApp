using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Repository.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("rooms");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(r => r.Description)
                .HasMaxLength(1000);
            builder.Property(r => r.PrivacyType)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(r => r.RoomType)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(r => r.PasswordHash)
                .HasMaxLength(255);
            builder.Property(r => r.CreatedBy)
                .IsRequired();
            builder.Property(r => r.CreatedAt)
                .IsRequired();
        }
    }
}
