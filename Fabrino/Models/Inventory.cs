// Models/Inventory.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabrino.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }

        [Required]
        public int FabricID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal QuantityInStock { get; set; } = 0;

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinimumStockLevel { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        // Navigation Property
        [ForeignKey("FabricID")]
        public virtual Fabric Fabric { get; set; }
    }
}