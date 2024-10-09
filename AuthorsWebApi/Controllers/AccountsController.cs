using AuthorsWebApi.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountsController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Register(UsersCredentialDTO usersCredential)
        {
            var user = new IdentityUser { UserName = usersCredential.Email, Email = usersCredential.Email };
            var result = await _userManager.CreateAsync(user, usersCredential.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return BuildToken(usersCredential);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Login(UsersCredentialDTO usersCredential)
        {
            var result = await _signInManager.PasswordSignInAsync(usersCredential.Email, usersCredential.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
                return BadRequest("Login incorrecto");

            return BuildToken(usersCredential);
        }

        private AuthenticationResponseDTO BuildToken(UsersCredentialDTO usersCredential)
        {
            var claims = new List<Claim>()
            {
                new Claim("Email", usersCredential.Email),
            };
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var credentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(1);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: credentials);

            return new AuthenticationResponseDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration,
            };
        }
    }
};