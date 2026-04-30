using ChatApp.Core.Models.Enums;

namespace ChatApp.Core.Models
{
    public class Role
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public RoleScope Scope { get; private set; }
        public Guid? RoomId { get; private set; }
        public Room? Room { get; private set; }
        public bool IsCustom { get; private set; }
        public bool IsSystemDefault { get; private set; }
        public byte Priority { get; private set; }
        public string Color { get; private set; } = string.Empty;
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
        public ICollection<UserSystemRole> UserSystemRoles { get; private set; } = new List<UserSystemRole>();
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();
        public DateTime CreatedAt { get; private set; }

        private Role() { }

        /// <summary> Tạo system role (SuperAdmin, Moderator, User...)</summary>
        public Role(string name, string description = "", byte priority = 0, string color = "", bool isSystemDefault = false)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Description = description;
            this.Scope = RoleScope.System;
            this.RoomId = null;
            this.Priority = priority;
            this.Color = color;
            this.IsCustom = false;
            this.IsSystemDefault = isSystemDefault;
            this.CreatedAt = DateTime.UtcNow;
        }

        /// <summary> Tạo room role (Owner, Admin, Member hoặc custom role trong phòng)</summary>
        public Role(string name, Guid roomId, string description = "", byte priority = 0, string color = "", bool isCustom = true)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Description = description;
            this.Scope = RoleScope.Room;
            this.RoomId = roomId;
            this.Priority = priority;
            this.Color = color;
            this.IsCustom = isCustom;
            this.IsSystemDefault = false;
            this.CreatedAt = DateTime.UtcNow;
        }

        public void Rename(string name) => this.Name = name;
        public void SetDescription(string description) => this.Description = description;
        public void SetColor(string color) => this.Color = color;
        public void SetPriority(byte priority) => this.Priority = priority;
    }
}