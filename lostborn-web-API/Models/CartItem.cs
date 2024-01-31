using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lostborn_backend.Models
{
    public class CartItem
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Foreign key for Cart relationship
        [Required]
        [ForeignKey("Cart")]
        public int CartID { get; set; }

        // Foreign key for Product relationship
        [Required]
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Carts Cart { get; set; }
        [JsonIgnore]
        public Products Product { get; set; }
    }
}
