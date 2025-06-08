using Fabrino.Models;
using Fabrino.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fabrino.Controllers
{
    /// <summary>
    /// Controller responsible for handling password recovery operations
    /// </summary>
    public class ForgotPasswordController
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the forgot password controller
        /// </summary>
        public ForgotPasswordController()
        {
            _db = new AppDbContext(); 
        }

        /// <summary>
        /// Retrieves the security question for a given username
        /// </summary>
        /// <param name="username">Username to lookup</param>
        /// <returns>Security question or null if user not found</returns>
        public string GetSecurityQuestion(string username)
        {
            return _db.Users
                .Where(u => u.username == username)
                .Select(u => u.security_question)
                .FirstOrDefault();
        }

        /// <summary>
        /// Validates the user's security answer
        /// </summary>
        /// <param name="username">Username of the account</param>
        /// <param name="answer">Security answer to validate</param>
        /// <returns>True if answer is correct, false otherwise</returns>
        public bool ValidateSecurityAnswer(string username, string answer)
        {
            string hashedAnswer = SecurityHelper.ComputeSha256Hash(answer);

            return _db.Users
                .Any(u => u.username == username &&
                          u.security_answer_hash == hashedAnswer);
        }

        /// <summary>
        /// Resets user's password after security validation
        /// </summary>
        /// <param name="username">Username of the account</param>
        /// <param name="newPassword">New password to set</param>
        /// <returns>True if password was reset successfully</returns>
        public bool ResetPassword(string username, string newPassword)
        {
            var user = _db.Users
                .FirstOrDefault(u => u.username == username);

            if (user == null)
                return false;

            user.password_hash = SecurityHelper.ComputeSha256Hash(newPassword);
            return _db.SaveChanges() > 0;
        }
    }
}