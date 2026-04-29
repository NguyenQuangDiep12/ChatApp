using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class Permission
    {
        public Guid Id { get; private set; }
        public string PermissionCode { get; private set; }
        public string Description { get; private set; }
        public string Scope { get; private set; }
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    }
}
