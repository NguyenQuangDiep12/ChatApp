using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable("attachments");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.MessageId)
                .IsRequired();
            builder.Property(a => a.FileUrl)
                .IsRequired()
                .HasMaxLength(1000);
            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(a => a.FileType)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(a => a.FileSize)
                .IsRequired();
            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.HasOne(a => a.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            // Query attachments theo message
            builder.HasIndex(a => a.MessageId);

            // Query theo type (image, video, file)
            builder.HasIndex(a => a.FileType);

            // Optional: tối ưu sort theo thời gian
            builder.HasIndex(a => a.CreatedAt);
        }
    }
}