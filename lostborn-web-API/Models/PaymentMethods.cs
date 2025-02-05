using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace lostborn_backend.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodID { get; set; } // Unique identifier for payment method

        [Required]
        public int UserID { get; set; } // Foreign key to Users table

        [ForeignKey("UserID")]
        public Users User { get; set; } // Navigation property to the Users table

        [Required]
        public string CardNumber { get; set; } // Masked card number, e.g., **** **** **** 1234

        [Required]
        public string CardHolderName { get; set; }

        [Required]
        public string ExpirationDate { get; set; } // MM/YY

        [Required]
        public string Cvv { get; set; }

        public bool IsDefault { get; set; } // Flag for default payment method
    }

}
