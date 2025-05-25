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

        [Required(ErrorMessage = "نام تامین‌کننده الزامی است")]
        [StringLength(100, ErrorMessage = "نام نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string Name { get; set; }

        [StringLength(100)]
        public string ContactPerson { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "شماره تماس نمی‌تواند بیش از 20 کاراکتر باشد")]
        public string Phone { get; set; }

        [StringLength(100, ErrorMessage = "ایمیل نمی‌تواند بیش از 100 کاراکتر باشد")]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(255, ErrorMessage = "آدرس نمی‌تواند بیش از 255 کاراکتر باشد")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessage = "شماره مالیاتی نمی‌تواند بیش از 50 کاراکتر باشد")]
        public string TaxNumber { get; set; }

        // Navigation Properties
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}