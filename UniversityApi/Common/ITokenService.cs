using UniversityApi.Models;

namespace UniversityApi.Common
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user, IList<string> roles);
        DateTime GetExpiration();
    }
}