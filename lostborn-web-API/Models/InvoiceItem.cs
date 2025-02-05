using System;
using lostborn_backend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lostborn_backend.Models
{
	public class InvoiceItem
	{
        [Key]
        public int InvoiceItemID { get; set; } // Unique identifier for each invoice item

        [Required]
        public int InvoiceID { get; set; } // Foreign key to the Invoice

        [ForeignKey("InvoiceID")]
        public Invoice Invoice { get; set; } // Navigation property to the invoice

        [Required]
        public int ProductID { get; set; } // Foreign key to the product

        [ForeignKey("ProductID")]
        public Products Product { get; set; } // Navigation property for the product

        [Required]
        public int Quantity { get; set; } // Quantity of the product

        [Required]
        public decimal UnitPrice { get; set; } // Price of a single unit of the product

        public decimal TotalPrice => Quantity * UnitPrice; // Total price for this item

        public InvoiceItem()
		{
		}
	}
}

