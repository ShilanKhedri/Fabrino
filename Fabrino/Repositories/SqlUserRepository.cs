using Fabrino.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// SQL Server implementation of the user repository interface
/// Handles all user-related database operations
/// </summary>
public class SqlUserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes repository with database context
    /// </summary>
    public SqlUserRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Checks if provided credentials match a user in the database
    /// </summary>
    /// <returns>True if credentials are valid</returns>
    public bool IsValidUser(string username, string passwordHash)
    {
        return _db.Users.Any(u => u.username == username && u.password_hash == passwordHash);
    }

    /// <summary>
    /// Updates the last login timestamp for a user to current time
    /// </summary>
    /// <returns>True if update was successful</returns>
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

    /// <summary>
    /// Retrieves user data without tracking changes
    /// </summary>
    /// <returns>User model if found, null otherwise</returns>
    public UserModel GetUserByUsername(string username)
    {
        return _db.Users.AsNoTracking().FirstOrDefault(u => u.username == username);
    }
}