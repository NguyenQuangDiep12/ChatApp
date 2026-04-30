using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Repository.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("permissions");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.PermissionCode)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(p => p.Description)
                .HasMaxLength(500);
            builder.Property(p => p.Scope)
                .IsRequired()
                .HasConversion<int>();

            builder.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.PermissionCode)
                .IsUnique();

            builder.HasIndex(p => p.Scope);
        }
    }
}