// Models/Supplier.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabrino.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string ContactPerson { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string TaxNumber { get; set; }

        // Navigation Properties
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}