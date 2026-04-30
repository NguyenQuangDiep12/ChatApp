using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class RoomMemberConfiguration : IEntityTypeConfiguration<RoomMember>
    {
        public void Configure(EntityTypeBuilder<RoomMember> builder)
        {
            builder.ToTable("room_members");
            builder.HasKey(rm => rm.Id);
            builder.Property(rm => rm.Id)
                .ValueGeneratedNever();

            builder.Property(rm => rm.RoomId)
                .IsRequired();
            builder.Property(rm => rm.UserId)
                .IsRequired();
            builder.Property(rm => rm.RoleId)
                .IsRequired();
            builder.Property(rm => rm.InviteTokenId);
            builder.Property(rm => rm.JoinedAt)
                .IsRequired();
            builder.Property(rm => rm.LastReadAt)
                .IsRequired();

            builder.HasOne(rm => rm.Room)
                .WithMany(r => r.RoomMembers)
                .HasForeignKey(rm => rm.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(rm => rm.User)
                .WithMany(u => u.RoomMembers)
                .HasForeignKey(rm => rm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: 1 user chỉ có 1 record trong 1 room
            builder.HasIndex(rm => new { rm.RoomId, rm.UserId })
                .IsUnique();
        }
    }
}