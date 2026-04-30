using ChatApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Repository.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(r => r.Description)
                .HasMaxLength(500);
            builder.Property(r => r.Scope)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(r => r.RoomId);
            builder.Property(r => r.IsCustom)
                .IsRequired();
            builder.Property(r => r.IsSystemDefault)
                .IsRequired();
            builder.Property(r => r.Priority)
                .IsRequired();
            builder.Property(r => r.Color)
                .HasMaxLength(20);
            builder.Property(r => r.CreatedAt)
                .IsRequired();
            
            builder.HasOne(r => r.Room)
                .WithMany(rm => rm.Roles)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.RolePermissions)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.UserSystemRoles)
                .WithOne()
                .HasForeignKey(usr => usr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.RoomMembers)
                .WithOne(rm => rm.Role)
                .HasForeignKey(rm => rm.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            // Tối ưu query theo room
            builder.HasIndex(r => r.RoomId);
            // Unique: name trong cùng scope + room
            builder.HasIndex(r => new { r.Name, r.RoomId, r.Scope })
                .IsUnique();

            // Check constraints
            // 1. Nếu là System role → RoomId phải null
            // 2. Nếu là Room role → RoomId phải NOT null
            builder.ToTable("roles",t =>
            {
                t.HasCheckConstraint(
                    "CK_Role_Scope_Room",
                    "(Scope = 0 AND RoomId IS NULL) OR (Scope = 1 AND RoomId IS NOT NULL)"
                );
            });
        }
    }
}
