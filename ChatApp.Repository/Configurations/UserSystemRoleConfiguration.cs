using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class UserSystemRoleConfiguration : IEntityTypeConfiguration<UserSystemRole>
    {
        public void Configure(EntityTypeBuilder<UserSystemRole> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();
            builder.Property(x => x.UserId)
                .IsRequired();
            builder.Property(x => x.RoleId)
                .IsRequired();
            builder.Property(x => x.AssignedBy)
                .IsRequired();
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(u => u.UserSystemRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Role)
                .WithMany(r => r.UserSystemRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.UserId, x.RoleId })
                .IsUnique();

            builder.HasIndex(x => x.RoleId);

            builder.ToTable("user_system_roles", t =>
            {
                t.HasCheckConstraint(
                    "CK_UserSystemRole_SystemScope",
                    "RoleId IN (SELECT Id FROM roles WHERE Scope = 0)"
                );
            });
        }
    }
}