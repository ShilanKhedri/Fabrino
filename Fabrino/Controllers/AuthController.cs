public class AuthController
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool Login(string username, string passwordHash)
    {
        var isValid = _userRepository.IsValidUser(username, passwordHash);
        /*if (isValid)
        {
            _userRepository.UpdateLastLogin(username);
        }*/
        return isValid;
    }
}