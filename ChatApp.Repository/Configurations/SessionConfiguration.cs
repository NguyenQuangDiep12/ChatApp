using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("sessions");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .ValueGeneratedNever();

            builder.Property(s => s.UserId)
                .IsRequired();
            builder.Property(s => s.Token)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false);
            builder.Property(s => s.DeviceInfo)
                .HasMaxLength(500);
            builder.Property(s => s.IsActive)
                .IsRequired();
            builder.Property(s => s.ExpiresAt)
                .IsRequired();
            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            // Query session theo token (auth middleware)
            builder.HasIndex(s => s.Token)
                .IsUnique();
            // Query session theo user
            builder.HasIndex(s => s.UserId);
            // Query active sessions
            builder.HasIndex(s => new { s.UserId, s.IsActive });
            // Query expiration cleanup job
            builder.HasIndex(s => s.ExpiresAt);
        }
    }
}