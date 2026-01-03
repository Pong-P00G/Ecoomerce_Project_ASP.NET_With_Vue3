using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Controllers.Common
{
    [ApiController]
    [Route("api/tokens")]
    public class TokenController : ControllerBase
    {
        private readonly IRefreshTokenService _service;

        public TokenController(IRefreshTokenService service)
        {
            _service = service;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var token = await _service.GetValidTokenAsync(refreshToken);
            if (token == null) return Unauthorized();

            // generate new JWT here
            return Ok(new { message = "New access token issued" });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(string refreshToken)
        {
            var token = await _service.GetValidTokenAsync(refreshToken);
            if (token == null) return NotFound();

            await _service.RevokeTokenAsync(token);
            return Ok("Token revoked");
        }
    }

}
