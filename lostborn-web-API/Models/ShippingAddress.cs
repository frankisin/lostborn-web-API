using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lostborn_backend.Models
{
    public class ShippingAddress
    {
        [Key]
        public int ShippingAddressID { get; set; } // Unique identifier for the shipping address

        [Required]
        public int UserID { get; set; } // Foreign key to Users table

        [ForeignKey("UserID")]
        public Users User { get; set; } // Navigation property to the Users table

        [Required]
        [StringLength(255)]
        public string StreetAddress { get; set; } // Street Address

        [Required]
        [StringLength(100)]
        public string City { get; set; } // City

        [Required]
        [StringLength(100)]
        public string State { get; set; } // State

        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; } // Zip Code

        public bool IsDefault { get; set; } // Flag to indicate if it's the default shipping address
    }
}
