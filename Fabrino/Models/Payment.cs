// Models/Payment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabrino.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentType { get; set; } // Purchase, Sale

        [Required]
        public int ReferenceID { get; set; } // PurchaseOrderID or OrderID

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // Cash, Credit, Bank Transfer

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Completed, Pending, Failed

        public string Notes { get; set; }
    }
}