using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using lostborn_backend.Models;

namespace lostborn_backend.Models
{
    public class Invoice
	{
        [Key]
        public int InvoiceID { get; set; } // Unique identifier for the invoice

        [Required]
        public int UserID { get; set; } // Foreign key to the user

        [ForeignKey("UserID")]
        public Users User { get; set; } // Navigation property for the user

        [Required]
        public DateTime InvoiceDate { get; set; } // Date of invoice generation

        [Required]
        public decimal TotalAmount { get; set; } // Total cost of the invoice

        public string InvoiceNumber { get; set; } // Unique invoice number (e.g., INV-20230916-001)

        public string PaymentStatus { get; set; } // Payment status: Paid, Pending, Failed, etc.

        public string TransactionId { get; set; } // Store the transaction ID from payment gateway


        // Collection of items associated with the invoice
        public List<InvoiceItem> InvoiceItems { get; set; }

        public Invoice()
		{
            //When obj is insantiated set InvoiceItems as list...
            InvoiceItems = new List<InvoiceItem>();
        }
	}
}

