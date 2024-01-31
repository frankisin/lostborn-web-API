using lostborn_backend.Helpers;
using lostborn_backend.Models;
using lostborn_web_API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace lostborn_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly TokenService _tokenService;
        private readonly JwtConfig _jwtConfig;

        public AuthController(DataContext context, TokenService tokenService, IOptions<JwtConfig> jwtConfig)
        {
            _context = context;
            _tokenService = tokenService;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                //Query User entity for Username...
                var user = await _context.Users.SingleOrDefaultAsync(u => u.username == model.Username); // return frankisin username

                //If we have a user...
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.password)) //verify hashed password attempt w hashed pass in entity...
                {
                    // Generate claims for the user (customize as needed)
                    var claims = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.username),
                        new Claim(ClaimTypes.Role, user.Role)
                        // Add additional claims as needed
                    });

                    // Generate a token
                    var token = _tokenService.GenerateToken(claims);

                    // Return the token
                    return Ok(new { Token = token });
                }

                // Return unauthorized if the login fails
                return Unauthorized();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal Server Error");
            }
        }
    
}
}
