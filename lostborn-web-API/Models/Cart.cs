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
}
