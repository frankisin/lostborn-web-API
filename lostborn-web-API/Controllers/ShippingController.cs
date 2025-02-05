using lostborn_backend.Helpers;
using lostborn_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using lostborn_web_API.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;

namespace lostborn_backend.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class LostBornController : Controller
{
    private readonly DataContext _context;
    private readonly TokenService _tokenService;
    private readonly JwtConfig _jwtConfig;

    public LostBornController(DataContext dataContext, TokenService tokenService, IOptions<JwtConfig> jwtConfig)
    {
        _context = dataContext;
        _tokenService = tokenService;
        _jwtConfig = jwtConfig.Value;
    }

    // API Call to get all employees...
    
    [HttpGet]
    public async Task<IActionResult> GetAllMembers()
    {
        try
        {
            // Check if the user has the 'admin' role
            var roleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value; // take a peak at the request header role...
            if (roleClaim != "admin")
            {
                return Forbid("Only admin can access this resource.");  // If the role is not 'admin', forbid access
            }

            // Proceed to get members or data as needed
            var res = await _context.PaymentMethods.ToListAsync();
            return Ok(res);
        }
        catch (Exception ex)
        {
            // Log error if necessary
            return NotFound(new { Error = ex.Message });
        }
    }

    // API Call to get member by Id..(READ)
    [HttpGet("{ID}")]
    public async Task<ActionResult<PaymentMethod>> GetMember(int ID)
    {
        if (_context.PaymentMethods == null)
        {
            return NotFound();
        }
        var member = await _context.PaymentMethods.FindAsync(ID);

        if (member == null)
        {
            return NotFound();
        }

        return member;
    }

    // API Call to add new entry...(CREATE)
    [HttpPost]
    public async Task<IActionResult> AddMember([FromBody] PaymentMethod member)
    {
        await _context.PaymentMethods.AddAsync(member);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMember), new { ID = member.PaymentMethodID }, member);
    }

    // API Call to update member using Id...(UPDATE)
    [HttpPut("{ID}")]
    public async Task<IActionResult> PutMember(int ID, [FromBody] PaymentMethod member)
    {
        if (ID != member.PaymentMethodID)
        {
            return BadRequest("Mismatched PaymentMethod ID");
        }

        _context.Entry(member).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MemberExists(ID))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        // You can create a custom response object or use an anonymous object
        return Ok(new
        {
            Message = "Record Updated successfully",
            UpdatedData = member
        });
    }

    // Helper function to check if a member is present..
    private bool MemberExists(int id)
    {
        return _context.PaymentMethods?.Any(e => e.PaymentMethodID == id) ?? false;
    }

    // API Call to delete a member using Id...(DELETE)
    [HttpDelete("{ID}")]
    public async Task<IActionResult> DeleteMember(int ID)
    {
        if (_context.PaymentMethods == null)
        {
            return NotFound();
        }

        var member = await _context.PaymentMethods.FindAsync(ID);

        if (member == null)
        {
            return NotFound();
        }

        _context.PaymentMethods.Remove(member);
        await _context.SaveChangesAsync();

        // Return the custom response
        return Ok(new
        {
            Message = "Record Deleted successfully",
            DeletedData = member
        });
    }
}
