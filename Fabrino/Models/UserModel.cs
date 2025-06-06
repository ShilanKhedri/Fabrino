using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fabrino.Models
{

    public class UserModel
    {

        public int id { get; set; }
        public string username { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? security_question { get; set; } = string.Empty;
        public string? security_answer_hash { get; set; } = string.Empty;
        public bool is_active { get; set; } = true;

        public DateTime? last_login { get; set; }
    }
}

