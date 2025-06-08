using Fabrino.Models;

/// <summary>
/// Interface defining user data access operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Validates user credentials against stored data
    /// </summary>
    /// <returns>True if credentials are valid</returns>
    bool IsValidUser(string username, string passwordHash);

    /// <summary>
    /// Retrieves user information by username
    /// </summary>
    /// <returns>User model if found, null otherwise</returns>
    public UserModel GetUserByUsername(string username);

    /// <summary>
    /// Updates the last login timestamp for a user
    /// </summary>
    /// <returns>True if update successful</returns>
    public bool UpdateLastLogin(string username);
}
