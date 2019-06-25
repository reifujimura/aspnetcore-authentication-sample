using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<object> Get()
        {
            foreach (var claim in User.Claims)
            {
                System.Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            return new
            {
                iss = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss)?.Value,
                jti = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value
            };
        }
    }
}
