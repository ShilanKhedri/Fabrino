using Fabrino.Models;
using System.Linq;

namespace Fabrino.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public UserModel GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.id == id);
        }

        public void UpdateUserInfo(UserModel user, string name, string email, string phone)
        {
            user.full_name = name;
            user.Email = email;
            user.Phone = phone;
            _context.SaveChanges();
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
