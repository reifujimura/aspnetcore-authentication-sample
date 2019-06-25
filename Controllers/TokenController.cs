using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationSample.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        readonly UserManager<IdentityUser> _userManager;

        public TokenController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Consumes("application/json")]
        public async Task<IActionResult> Get([FromBody]IdentityModel info)
        {
            if (ModelState.IsValid)
            {
                var identityUser = _userManager.Users.FirstOrDefault(x => x.UserName == info.UserName);
                var isVailed = await _userManager.CheckPasswordAsync(identityUser, info.Password);
                if (isVailed)
                {
                    return Ok(CreateToken(identityUser.UserName));
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post() => Ok(CreateToken(_userManager.GetUserName(User)));

        private dynamic CreateToken(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, userName)
            };
            var expires = DateTime.UtcNow.AddMinutes(1);
            var token = new JwtSecurityToken(
                issuer: AppConfig.SiteUrl,
                audience: AppConfig.SiteUrl,
                claims: claims,
                expires: expires,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(AppConfig.SecretKey),
                    SecurityAlgorithms.HmacSha256
                )
            );
            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = expires
            };
        }
    }
}
