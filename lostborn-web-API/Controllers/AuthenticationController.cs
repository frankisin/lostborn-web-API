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
using Microsoft.AspNetCore.Authorization;

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
                var user = await _context.Users.SingleOrDefaultAsync(u => u.username == model.Username); // return username

                //If we have a user...
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.password)) //verify hashed password attempt w hashed pass in entity...
                {
                    // Generate claims for the user...
                    var claims = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.username),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("UserID", user.ID.ToString())
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
        // Add an endpoint to get user profile
        [HttpGet("profile")]
        [Authorize]  // Ensures the request is authenticated
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Extract the JWT from the Authorization header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Validate and decode the token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Extract UserID from JWT claims
                var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userIdClaim == null)
                    return Unauthorized("Invalid token.");

                // Convert the UserID to an integer
                int userId = int.Parse(userIdClaim.Value);

                // Fetch the user from the database using the UserID
                var user = await _context.Users.SingleOrDefaultAsync(u => u.ID == userId);
                if (user == null)
                    return NotFound("User not found.");

                // Return the user's profile data
                return Ok(new
                {
                    user.ID,
                    user.firstName,
                    user.lastName,
                    user.username,
                    user.email,
                    user.streetAddress,
                    user.city,
                    user.zipCode,
                    user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] TokenModel tokenModel)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

                tokenHandler.ValidateToken(tokenModel.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Optional: reduce clock skew for more strict validation
                }, out SecurityToken validatedToken);

                // If token is successfully validated, return true
                return Ok(new { valid = true });
            }
            catch (Exception)
            {
                // If validation fails, return false
                return Ok(new { valid = false });
            }
        }

        public class TokenModel
        {
            public string Token { get; set; }
        }


    }
}
