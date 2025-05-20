using Fabrino.Models;
using System.Windows;

public class SignUpController
{
    private readonly ISignUpRepository _repository;

    public SignUpController()
    {
        _repository = new EfSignUpRepository(new AppDbContext());
    }

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