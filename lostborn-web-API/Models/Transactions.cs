using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace lostborn_backend.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int QuantityChange { get; set; }
        public string TransactionType { get; set; }  // SALE, RETURN, RESTOCK
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        [ForeignKey("InvoiceID")]
        public int? InvoiceID { get; set; }  // Nullable if not linked to an invoice
    }

}

