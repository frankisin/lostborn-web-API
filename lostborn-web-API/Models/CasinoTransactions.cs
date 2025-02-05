using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lostborn_backend.Models
{
    public class CasinoTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string GameType { get; set; }
        public decimal BetAmount { get; set; }
        public decimal Multiplier { get; set; }
        public decimal Winnings { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Users User { get; set; } // Navigation property
    }
}
