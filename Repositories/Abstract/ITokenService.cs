using AuthApi.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthApi.Repositories.Abstract
{
    public interface ITokenService
    {
        TokenResponse GetToken(IdentityUser user, IList<string> userRoles);
        TokenResponse GetrefToken(IEnumerable<Claim> claim);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    }
}
