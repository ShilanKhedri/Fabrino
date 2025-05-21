// Models/PurchaseOrderItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabrino.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int PurchaseOrderItemID { get; set; }

        [Required]
        public int PurchaseOrderID { get; set; }

        [Required]
        public int FabricID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ReceivedQuantity { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; private set; } // Quantity * UnitPrice

        // Navigation Properties
        [ForeignKey("PurchaseOrderID")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        [ForeignKey("FabricID")]
        public virtual Fabric Fabric { get; set; }
    }
}