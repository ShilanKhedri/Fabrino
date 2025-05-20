using Fabrino.Models;
using Fabrino.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fabrino.Controllers
{
    public class ForgotPasswordController
    {
        private readonly AppDbContext _db;

        public ForgotPasswordController()
        {
            _db = new AppDbContext(); 
        }

        public string GetSecurityQuestion(string username)
        {
            return _db.Users
                .Where(u => u.username == username)
                .Select(u => u.security_question)
                .FirstOrDefault();
        }

        public bool ValidateSecurityAnswer(string username, string answer)
        {
            string hashedAnswer = SecurityHelper.ComputeSha256Hash(answer);

            return _db.Users
                .Any(u => u.username == username &&
                          u.security_answer_hash == hashedAnswer);
        }

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