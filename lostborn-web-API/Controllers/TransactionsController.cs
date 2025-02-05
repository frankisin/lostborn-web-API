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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly TokenService _tokenService;
        private readonly JwtConfig _jwtConfig;

        public TransactionsController(DataContext context, TokenService tokenService, IOptions<JwtConfig> jwtConfig)
        {
            _context = context;
            _tokenService = tokenService;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
                return BadRequest("Transaction is null");

            // Add the transaction to the database
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        // GET: api/transactions/{productId}
        // Get all transactions for a specific product
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetTransactionsForProduct(int productId)
        {
            var transactions = await _context.Transactions
                                             .Where(t => t.ProductID == productId)
                                             .ToListAsync();

            if (transactions == null || transactions.Count == 0)
                return NotFound("No transactions found for this product");

            return Ok(transactions);
        }

        // GET: api/transactions
        // Get all transactions (optional: add filters for date, type, etc.)
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }
    }
}
