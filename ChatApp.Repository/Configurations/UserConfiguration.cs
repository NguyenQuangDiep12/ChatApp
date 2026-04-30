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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);
            builder.HasIndex(x => x.Email)
                .IsUnique();
            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.AvatarUrl)
                .HasMaxLength(255);
            builder.Property(x => x.UserStatus)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(x => x.LastSeen);
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(u => u.UserSettings)
                .WithOne() // unidirectional 
                .HasForeignKey<UserSetting>("UserId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.Sessions)
                .WithOne()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.Notifications)
                .WithOne()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.MessageReads)
                .WithOne()
                .HasForeignKey(s => s.UserId);
            builder.HasMany(u => u.UserSystemRoles)
                .WithOne()
                .HasForeignKey(s => s.UserId);
            builder.HasMany(u => u.InviteTokens)
                .WithOne()
                .HasForeignKey(s => s.CreatedBy);
            builder.HasMany(u => u.Messages)
                .WithOne()
                .HasForeignKey(s => s.SenderId);
            builder.HasMany(u => u.RoomMembers)
                .WithOne(x => x.User) // bidirection
                .HasForeignKey(s => s.UserId);

            builder.Ignore(u => u.Rooms); // bo qua quan he 
        }
    }
}
