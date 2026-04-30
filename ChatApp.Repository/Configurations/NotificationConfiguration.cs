using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
                .ValueGeneratedNever();

            builder.Property(n => n.UserId)
                .IsRequired();
            builder.Property(n => n.MessageId)
                .IsRequired();
            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(n => n.IsRead)
                .IsRequired();
            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(n => n.Message)
                .WithMany(m => m.Notifications)
                .HasForeignKey(n => n.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes 
            // Query danh sách thông báo theo user
            builder.HasIndex(n => n.UserId);
            // Lọc unread
            builder.HasIndex(n => new { n.UserId, n.IsRead });
            // Sort theo thời gian
            builder.HasIndex(n => n.CreatedAt);
        }
    }
}