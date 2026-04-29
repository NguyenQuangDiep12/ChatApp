using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class RolePermission
    {
        public Guid Id { get; private set; }
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = null!;
        public Guid PermissionId { get; private set; }
        public Permission Permission { get; private set; } = null!;
        
        private RolePermission() { }
        public RolePermission(Permission Permission)
        {
            this.Id = Guid.NewGuid();
            this.Permission = Permission;
        }
    }
}
