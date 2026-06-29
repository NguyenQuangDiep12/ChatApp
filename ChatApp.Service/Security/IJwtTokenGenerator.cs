using ChatApp.Core.Models;

namespace ChatApp.Service.Security
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
