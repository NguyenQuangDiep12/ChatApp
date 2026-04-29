using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class Role
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public RoleScope RoleScope { get; private set; }
        public bool IsCustom { get; private set; }
        public bool IsSystemDefault { get; private set; }
        public byte Priority { get; private set; }
        public string Color { get; private set; } = string.Empty;
        public ICollection<UserSystemRole> UserSystemRoles { get; private set; } = new List<UserSystemRole>();
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();
        public DateTime CreatedAt { get; private set; }
        private Role() { }

        public Role(string Name, string Description = "", byte Priority = 0, string Color = "")
        {
            this.Id = Guid.NewGuid();
            this.Name = Name;
            this.Description = Description;
            this.Priority = Priority;
            this.Color = Color;
            this.CreatedAt = DateTime.UtcNow;
        }
        
        
    }
}
