using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("messages");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(4000);
            builder.Property(m => m.Type)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(m => m.IsEdited)
                .IsRequired();
            builder.Property(m => m.IsDeleted)
                .IsRequired();
            builder.Property(m => m.CreatedAt)
                .IsRequired();
            builder.Property(m => m.UpdatedAt)
                .IsRequired();
            builder.Property(m => m.SenderId)
                .IsRequired();
            builder.Property(m => m.RoomId)
                .IsRequired();
            builder.Property(m => m.ReplyToId);

            // Sender (User)
            builder.HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Room
            builder.HasOne(m => m.Room)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Self reference (Reply)
            builder.HasOne(m => m.ReplyTo)
                .WithMany(m => m.Replies)
                .HasForeignKey(m => m.ReplyToId)
                .OnDelete(DeleteBehavior.Restrict);

            // Attachments
            builder.HasMany(m => m.Attachments)
                .WithOne()
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // MessageReads
            builder.HasMany(m => m.ReadMessages)
                .WithOne(mr => mr.Message)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notifications
            builder.HasMany(m => m.Notifications)
                .WithOne(n => n.Message)
                .HasForeignKey(n => n.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            // Query theo room (timeline)
            builder.HasIndex(m => m.RoomId);
            // Paging theo thời gian
            builder.HasIndex(m => new { m.RoomId, m.CreatedAt });
            // Query theo sender
            builder.HasIndex(m => m.SenderId);
            // Reply lookup
            builder.HasIndex(m => m.ReplyToId);
        }
    }
}