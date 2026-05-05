using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task<Permission?> GetByCodeAsync(string code);
        Task<IEnumerable<Permission>> GetByScopeAsync(string scope);
    }
}
