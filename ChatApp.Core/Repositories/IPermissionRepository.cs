using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;

namespace ChatApp.Core.Repositories
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task<Permission?> GetByCodeAsync(string code);
        Task<IEnumerable<Permission>> GetByScopeAsync(RoleScope scope);
    }
}
