/// <summary>
/// Controller responsible for handling user authentication operations
/// </summary>
public class AuthController
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the authentication controller
    /// </summary>
    /// <param name="userRepository">Repository for user-related operations</param>
    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Validates user credentials and performs login
    /// </summary>
    /// <param name="username">User's username</param>
    /// <param name="passwordHash">Hashed password for security</param>
    /// <returns>True if login successful, false otherwise</returns>
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