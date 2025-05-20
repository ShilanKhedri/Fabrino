using Fabrino.Models;

public interface IUserRepository
{
    bool IsValidUser(string username, string passwordHash);
    public UserModel GetUserByUsername(string username);
    public bool UpdateLastLogin(string username);
}
