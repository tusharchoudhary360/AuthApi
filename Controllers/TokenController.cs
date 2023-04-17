using AuthApi.Models.Domain;
using AuthApi.Models.DTO;
using AuthApi.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly DatabaseContext _ctx;
        private readonly ITokenService _service;
        public TokenController(DatabaseContext ctx, ITokenService service)
        {
            this._ctx = ctx;
            this._service = service;

        }

        [HttpPost]
        public IActionResult Refresh(RefreshTokenRequest tokenApiModel)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Status(400, "Please pass all the Fields", null));
            }
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = _service.GetPrincipalFromExpiredToken(accessToken);
            var usename = principal.FindFirstValue(ClaimTypes.Email);
            var user = _ctx.TokenInfo.SingleOrDefault(u => u.Usename == usename);
            if (user is null)
            {
                return Ok(new Status(400, "User Not Found check parameters again", null));
            }
            if (user.RefreshToken != refreshToken)
            {
                return Ok(new Status(400, "Invalid Refresh token user maybe logout", null));
            }
            if (user.RefreshTokenExpiry <= DateTime.Now)
            {
                return Ok(new Status(400, "Refresh token not expire yet", null));
            }
       
            var newAccessToken = _service.GetrefToken(principal.Claims);
            var newRefreshToken = _service.GetRefreshToken();
            user.RefreshToken = newRefreshToken;
            _ctx.SaveChanges();
            return Ok(new Status (200,"Generate token Success",new RefreshTokenRequest()
            {
                AccessToken = newAccessToken.TokenString,
                RefreshToken = newRefreshToken
            }));
        }

        [HttpPost, Authorize]
        public IActionResult Logout()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = _ctx.TokenInfo.SingleOrDefault(u => u.Usename == userEmail);
                if (user is null)
                {
                    return Ok(new Status(400, "User Not Found check token again", null));
                }
                
                user.RefreshToken = null;
                _ctx.SaveChanges();
                return Ok(new Status(200, "Successfully Logged out ", null));
            }
            catch (Exception ex)
            {
                return Ok(new Status(400, "Server Error", null));
            }
        }

    }
}
