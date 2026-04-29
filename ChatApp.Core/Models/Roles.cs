using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class Roles
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public RoleScope RoleScope { get; private set; }
        public bool IsCustom { get; private set; }
        public bool IsSystemDefault { get; private set; }
        public byte Priority { get; private set; }
        public string Color { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        private Roles() { }

        public Roles(string Name, string Description = "", byte Priority = 0, string Color = "")
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
