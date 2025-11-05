using EVOpsPro.Repositories.KhiemNVD.Models;
using EVOpsPro.Servcies.KhiemNVD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EVOpsPro.WebAPI.KhiemNVD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemUserAccountsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SystemUserAccountService _userAccountsService; //// don't forget add Dependency Injection in program.cs

        public SystemUserAccountsController(IConfiguration config, SystemUserAccountService userAccountsService)
        {
            _config = config;
            _userAccountsService = userAccountsService;     //// Add DJ
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _userAccountsService.GetUserAccount(request.UserName, request.Password);

            if (user == null || user.Result == null)
                return Unauthorized();

            var token = GenerateJSONWebToken(user.Result);

            return Ok(token);
        }

        [HttpGet]
        [Authorize(Roles = "1,2")]
        public async Task<ActionResult<IEnumerable<SystemUserAccount>>> GetAll()
        {
            var users = await _userAccountsService.GetAllAsync();
            return Ok(users);
        }

        private string GenerateJSONWebToken(SystemUserAccount systemUserAccount)
        {
            if(systemUserAccount.RoleId == 3)
            {
                return string.Empty;
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"]
                    , _config["Jwt:Audience"]
                    , new Claim[]
                    {
                    new(ClaimTypes.Name, systemUserAccount.UserName),
                    //new(ClaimTypes.Email, systemUserAccount.Email),
                    new(ClaimTypes.Role, systemUserAccount.RoleId.ToString()),
                    },
                    expires: DateTime.Now.AddMinutes(180),
                    signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public sealed record LoginRequest(string UserName, string Password);
    }
}
