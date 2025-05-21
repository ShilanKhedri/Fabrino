// Models/PurchaseOrder.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabrino.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderID { get; set; }

        [Required]
        public int SupplierID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? ExpectedDeliveryDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public string Notes { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; }

        // Navigation Properties
        [ForeignKey("SupplierID")]
        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }
}