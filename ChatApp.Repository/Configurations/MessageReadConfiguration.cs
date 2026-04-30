using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class MessageReadConfiguration : IEntityTypeConfiguration<MessageRead>
    {
        public void Configure(EntityTypeBuilder<MessageRead> builder)
        {
            builder.ToTable("message_reads");
            builder.HasKey(mr => mr.Id);
            builder.Property(mr => mr.Id)
                .ValueGeneratedNever();

            builder.Property(mr => mr.UserId)
                .IsRequired();
            builder.Property(mr => mr.MessageId)
                .IsRequired();
            builder.Property(mr => mr.ReadAt)
                .IsRequired();

            builder.HasOne(mr => mr.User)
                .WithMany(u => u.MessageReads)
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(mr => mr.Message)
                .WithMany(m => m.ReadMessages)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint
            builder.HasIndex(mr => new { mr.UserId, mr.MessageId })
                .IsUnique();
            // Index hỗ trợ query
            builder.HasIndex(mr => mr.MessageId);
        }
    }
}