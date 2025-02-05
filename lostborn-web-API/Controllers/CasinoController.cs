using lostborn_backend.Helpers;
using lostborn_backend.Models;
using lostborn_web_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lostborn_backend.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class CasinoController : ControllerBase
    {
        private readonly DataContext _context;

        public CasinoController(DataContext context)
        {
            _context = context;
        }

        // Add a new transaction
        [HttpPost("addTransaction")]
        public async Task<IActionResult> AddTransaction([FromBody] CasinoTransaction transactionDto)
        {
            var user = await _context.Users.FindAsync(transactionDto.UserId);
            if (user == null)
                return NotFound("User not found");

            var transaction = new CasinoTransaction
            {
                UserId = transactionDto.UserId,
                GameType = transactionDto.GameType,
                BetAmount = transactionDto.BetAmount,
                Multiplier = transactionDto.Multiplier,
                Winnings = transactionDto.Winnings,
                BalanceAfter = transactionDto.BalanceAfter
            };

            _context.CasinoTransactions.Add(transaction);
            user.userBalance = transactionDto.BalanceAfter; // Update user's balance
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        // Fetch transaction history for a user
        [HttpGet("getTransactions/{userId}")]
        public async Task<IActionResult> GetTransactions(int userId)
        {
            var transactions = await _context.CasinoTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();

            return Ok(transactions);
        }
    }
}
