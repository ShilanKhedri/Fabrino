using Fabrino.Models;
using System.Linq;

namespace Fabrino.Services
{
    /// <summary>
    /// Core service for user management operations including profile updates and authentication
    /// </summary>
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Basic user lookup and retrieval operations
        public UserModel GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.id == id);
        }

        // Handles user profile information updates
        public void UpdateUserInfo(UserModel user, string name, string email, string phone)
        {
            var dbUser = _context.Users.FirstOrDefault(u => u.id == user.id);
            if (dbUser != null)
            {
                dbUser.full_name = name;
                dbUser.Email = email;
                dbUser.Phone = phone;
                _context.Users.Update(dbUser);
            }
        }

        public bool ChangePassword(UserModel user, string currentPass, string newPass)
        {
            if (user.password_hash != currentPass)
                return false;

            user.password_hash = newPass;
            _context.SaveChanges();
            return true;
        }

        public void AddUser(UserModel user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
