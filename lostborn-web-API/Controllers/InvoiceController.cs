using lostborn_backend.Helpers;
using lostborn_backend.Models;
using lostborn_web_API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace lostborn_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly TokenService _tokenService;
        private readonly JwtConfig _jwtConfig;

        public InvoiceController(DataContext context, TokenService tokenService, IOptions<JwtConfig> jwtConfig)
        {
            _context = context;
            _tokenService = tokenService;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpGet("AllInvoice")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            return await _context.Invoices.ToListAsync();
        }
        [Authorize]
        [HttpGet("Invoice")]
        public async Task<ActionResult<List<Invoice>>> GetInvoice()
        {
            try
            {
               
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    return Unauthorized("Missing Authorization header.");
                }

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");


                // Validate the presence of the token
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Missing or invalid Authorization header.");

                // Validate and decode the token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

                ClaimsPrincipal claimsPrincipal;
                try
                {
                    claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                }
                catch (Exception)
                {
                    return Unauthorized("Invalid or expired token.");
                }

                // Extract UserID from JWT claims
                var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized("Invalid UserID in token.");

                // Fetch the user from the database
                var user = await _context.Users.SingleOrDefaultAsync(u => u.ID == userId);
                if (user == null)
                    return NotFound("User not found.");

                // Fetch the invoices for the user
                var invoices = await _context.Invoices
                    .Include(i => i.InvoiceItems) // Include related invoice items
                    .ThenInclude(ii => ii.Product) // Include product details for each item
                    .Where(i => i.UserID == userId) // Filter by UserID
                    .ToListAsync();

                if (invoices == null || invoices.Count == 0)
                {
                    return NotFound("No invoices found for this user.");
                }

                // Return the user's invoice data
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                // Log the exception (optional) for debugging
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }




        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice)
        {
            // Generate a unique invoice number before saving
            invoice.InvoiceNumber = GenerateInvoiceNumber();
            invoice.InvoiceDate = DateTime.Now;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceID }, invoice);
        }
        //API Call to update invoice using Id...(UPDATE)
        [HttpPut]
        public async Task<IActionResult> UpdateInvoice(Invoice invoice)
        {
            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // if (!MemberExists(id)) { return NotFound(); }
                if (!InvoiceExists(invoice.InvoiceID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
                throw;
            }

            // Fetch the updated data from the database
            var updatedMember = await _context.Invoices.FindAsync(invoice.InvoiceID);

            // You can create a custom response object or use an anonymous object
            var response = new
            {
                Message = "User Cart Updated successfully",
                UpdatedData = updatedMember
            };

            // Return the custom response
            return Ok(response);
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(c => c.InvoiceID == id);
        }
        private string GenerateInvoiceNumber()
        {
            var datePart = DateTime.Now.ToString("yyyyMMdd"); // e.g., 20230916
            var randomPart = new Random().Next(100, 999); // e.g., 001-999
            return $"INV-{datePart}-{randomPart}";
        }
        //update order status of invoice.
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] string newStatus)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Update the payment status
            invoice.PaymentStatus = newStatus;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated invoice
            return Ok(new { message = "Payment status updated", invoice });
        }
    }
}
