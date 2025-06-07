using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fabrino.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Response { get; set; } // پاسخ ادمین
        public string Status { get; set; } // "باز", "پاسخ داده شده", "بسته شده"
        public DateTime CreatedAt { get; set; }

        // ارتباط با کاربر
        public int UserId { get; set; }
        public UserModel User { get; set; }
    }

}
