using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("role_permissions");
            builder.HasKey(rp => rp.Id);
            builder.Property(rp => rp.Id)
                .ValueGeneratedNever();

            builder.Property(rp => rp.RoleId)
                .IsRequired();
            builder.Property(rp => rp.PermissionId)
                .IsRequired();

            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint (QUAN TRỌNG)
            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();
            // Index hỗ trợ query
            builder.HasIndex(rp => rp.PermissionId);
        }
    }
}