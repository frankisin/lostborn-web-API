using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lostborn_backend.Models
{
    public class Carts
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [JsonIgnore]
        public Users User { get; set; }

        // Collection of items in the cart
        public List<CartItem> CartItems { get; set; }

        public Carts()
        {
            CartItems = new List<CartItem>();
        }
        
    }
    //Data Transfer Object (DTO) for Payment requests and response...
    public class PaymentRequest
    {
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string Cvv { get; set; }
        public decimal Amount { get; set; }
    }
    //helper class that defines mock response 
    public class PaymentResponse
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }  // Unique identifier for the transaction if successful
        public string ErrorMessage { get; set; }  // Error message if the payment failed
    }

    public class CheckoutModel
    {
        // User identifier if needed, could be obtained from session or token
        public int UserId { get; set; }


        // Payment details
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string Cvv { get; set; }

        // Optional: Add more fields if required for your checkout process
        // For example, shipping address details
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Total amount to be charged; 
        // This could be calculated on the server-side based on the cart contents to avoid tampering.
        public decimal TotalAmount { get; set; }
    }

}
