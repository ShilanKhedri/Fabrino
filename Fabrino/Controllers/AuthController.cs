using Fabrino.Models;

namespace Fabrino.Controllers
{
    public class AuthController
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool IsValidUser(UserModel user)
        {
            return _userRepository.IsValidUser(user.username, user.password_hash);
        }
    }
}
