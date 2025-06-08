using Fabrino.Models;
using System.Windows;

/// <summary>
/// Controller responsible for handling user registration operations
/// </summary>
public class SignUpController
{
    private readonly ISignUpRepository _repository;

    /// <summary>
    /// Initializes a new instance of the signup controller with default repository
    /// </summary>
    public SignUpController()
    {
        _repository = new EfSignUpRepository(new AppDbContext());
    }

    /// <summary>
    /// Registers a new user after validating username availability
    /// </summary>
    /// <param name="user">User model containing registration details</param>
    /// <returns>True if registration successful, false if username taken</returns>
    public bool RegisterUser(UserModel user)
    {
        if (_repository.IsUsernameTaken(user.username))
        {
            MessageBox.Show("نام کاربری قبلاً استفاده شده است!");
            return false;
        }

        return _repository.RegisterUser(user);
    }
}