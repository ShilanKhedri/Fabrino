using Fabrino.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabrino.Repositories
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public SqlUserRepository(AppDbContext db)
        {
            _db = db;
        }

        public bool IsValidUser(string username, string passwordHash)
        {
            return _db.Users
                .Any(u => u.username == username && u.password_hash == passwordHash);
        }
    }
}