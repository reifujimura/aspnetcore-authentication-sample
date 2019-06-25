using System.Threading.Tasks;
using AuthenticationSample.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSample.Controllers
{
    [Route("/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly SignInManager<IdentityUser> _signInManager;
        readonly UserManager<IdentityUser> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("signup")]
        [Consumes("application/json")]
        public async Task<IActionResult> SignUp(IdentityModel input)
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser { UserName = input.UserName };
                var createResult = await _userManager.CreateAsync(identityUser, input.Password);
                if (createResult.Succeeded && !string.IsNullOrWhiteSpace(input.Role))
                {
                    if (!await _roleManager.RoleExistsAsync(input.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(input.Role));
                    }
                    createResult = await _userManager.AddToRoleAsync(identityUser, input.Role);
                }
                return Ok(createResult.Succeeded.ToString());
            }
            return BadRequest();
        }

        [HttpPost("signin")]
        [Consumes("application/json")]
        public async Task<IActionResult> SignIn(IdentityModel input)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, false, false);
                return Ok(signInResult.Succeeded.ToString());
            }
            return BadRequest();
        }

        [HttpGet("signout")]
        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
