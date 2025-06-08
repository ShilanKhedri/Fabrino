using System.Security.Cryptography;
using System.Text;

namespace Fabrino.Helpers
{
    /// <summary>
    /// Provides security-related utility functions for the application
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Computes SHA-256 hash of input string for secure password storage
        /// </summary>
        /// <param name="rawData">The raw string to be hashed</param>
        /// <returns>Hexadecimal string representation of the hash</returns>
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
