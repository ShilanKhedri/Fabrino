// Models/InventoryTransaction.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabrino.Models
{
    public class InventoryTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public int FabricID { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } // Purchase, Sale, Adjustment

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Quantity { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public int? ReferenceID { get; set; } // PurchaseOrderID or OrderID

        [StringLength(20)]
        public string ReferenceType { get; set; } // 'Purchase', 'Sale'

        public string Notes { get; set; }

        // Navigation Property
        [ForeignKey("FabricID")]
        public virtual Fabric Fabric { get; set; }
    }
}