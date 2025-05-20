using Fabrino.Models;

public interface ISignUpRepository
{
    bool RegisterUser(UserModel user);
    bool IsUsernameTaken(string username);
}