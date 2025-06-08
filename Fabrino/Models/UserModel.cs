using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fabrino.Models
{
    /// <summary>
    /// Represents a user in the system with authentication and profile information
    /// </summary>
    public class UserModel
    {
        /// <summary>Unique identifier for the user</summary>
        public int id { get; set; }

        /// <summary>Username for login</summary>
        public string username { get; set; } = string.Empty;

        /// <summary>Hashed password for security</summary>
        public string password_hash { get; set; } = string.Empty;

        /// <summary>User's full name</summary>
        public string full_name { get; set; } = string.Empty;

        /// <summary>User's role in the system (e.g., admin, user)</summary>
        public string role { get; set; } = string.Empty;

        /// <summary>Account creation timestamp</summary>
        public DateTime created_at { get; set; }

        /// <summary>Optional email address</summary>
        public string? Email { get; set; }

        /// <summary>Optional phone number</summary>
        public string? Phone { get; set; }

        /// <summary>Security question for password recovery</summary>
        public string? security_question { get; set; } = string.Empty;

        /// <summary>Hashed answer to security question</summary>
        public string? security_answer_hash { get; set; } = string.Empty;

        /// <summary>Whether the account is currently active</summary>
        public bool is_active { get; set; } = true;

        /// <summary>Timestamp of last successful login</summary>
        public DateTime? last_login { get; set; }
    }
}

