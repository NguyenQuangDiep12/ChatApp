using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IPermissionService : IService<Permission>
    {
        Task<Permission?> GetByCodeAsync(string code);
        Task<IEnumerable<Permission>> GetByScopeAsync(string scope);
    }
}