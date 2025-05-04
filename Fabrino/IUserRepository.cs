public interface IUserRepository
{
    bool IsValidUser(string username, string passwordHash);
}
