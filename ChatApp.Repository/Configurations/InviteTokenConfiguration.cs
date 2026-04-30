using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class InviteTokenConfiguration : IEntityTypeConfiguration<InviteToken>
    {
        public void Configure(EntityTypeBuilder<InviteToken> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .ValueGeneratedNever();

            builder.Property(i => i.RoomId)
                .IsRequired();
            builder.Property(i => i.CreatedBy)
                .IsRequired();
            builder.Property(i => i.Token)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
            builder.Property(i => i.Note)
                .HasMaxLength(500);
            builder.Property(i => i.IsActive)
                .IsRequired();
            builder.Property(i => i.MaxUsage)
                .IsRequired();
            builder.Property(i => i.UseCount)
                .IsRequired();
            builder.Property(i => i.ExpireAt)
                .IsRequired();
            builder.Property(i => i.CreatedAt)
                .IsRequired();

            // Room
            builder.HasOne(i => i.Room)
                .WithMany(r => r.InviteTokens)
                .HasForeignKey(i => i.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Creator (User)
            builder.HasOne(i => i.Creator)
                .WithMany()
                .HasForeignKey(i => i.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // RoomMembers
            builder.HasMany(i => i.RoomMembers)
                .WithOne(rm => rm.InviteToken)
                .HasForeignKey(rm => rm.InviteTokenId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define table name and constrainst the number of use token
            builder.ToTable("invite_tokens", t =>
            {
                // UseCount không được vượt MaxUsage
                t.HasCheckConstraint(
                    "CK_InviteToken_Usage",
                    "\"UseCount\" <= \"MaxUsage\""
                );
            });

            // Indexes
            // Token phải unique
            builder.HasIndex(i => i.Token)
                .IsUnique();
            // Query theo room
            builder.HasIndex(i => i.RoomId);
            // Query token còn dùng được
            builder.HasIndex(i => new { i.RoomId, i.IsActive, i.ExpireAt });
        }
    }
}