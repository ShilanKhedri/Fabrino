using Fabrino.Models;
using Microsoft.EntityFrameworkCore;

public class SqlUserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public SqlUserRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool IsValidUser(string username, string passwordHash)
    {
        return _db.Users.Any(u => u.username == username && u.password_hash == passwordHash);
    }

    public bool UpdateLastLogin(string username)
    {
        var user = _db.Users.FirstOrDefault(u => u.username == username);
        if (user != null)
        {
            user.last_login = DateTime.Now;
            return _db.SaveChanges() > 0;
        }
        return false;
    }

    public UserModel GetUserByUsername(string username)
    {
        return _db.Users.AsNoTracking().FirstOrDefault(u => u.username == username);
    }
}