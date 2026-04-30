using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
    {
        public void Configure(EntityTypeBuilder<UserSetting> builder)
        {
            builder.ToTable("user_settings");
            builder.HasKey(us => us.UserId);
            builder.Property(us => us.UserId)
                .ValueGeneratedNever();

            builder.Property(us => us.ShowOnlineStatus)
                .IsRequired();
            builder.Property(us => us.ShowLastSeen)
                .IsRequired();
            builder.Property(us => us.SendReadReceipt)
                .IsRequired();
            builder.Property(us => us.CreatedAt)
                .IsRequired();

            builder.HasOne(us => us.User)
                .WithOne(u => u.UserSettings)
                .HasForeignKey<UserSetting>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}