using Xunit;
using Moq;
using Fabrino.Models;
using Fabrino.Controllers;

namespace Fabrino.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public void IsValidUser_WithCorrectCredentials_ReturnsTrue()
        {
            var user = new UserModel { username = "shilan", password_hash = "correctHash" };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.IsValidUser("shilan", "correctHash")).Returns(true);

            var controller = new AuthController(mockRepo.Object);

            var result = controller.IsValidUser(user);

            Assert.True(result);
        }

        [Fact]
        public void IsValidUser_WithWrongCredentials_ReturnsFalse()
        {
            var user = new UserModel { username = "shilan", password_hash = "wrongHash" };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.IsValidUser("shilan", "wrongHash")).Returns(false);

            var controller = new AuthController(mockRepo.Object);

            var result = controller.IsValidUser(user);

            Assert.False(result);
        }
    }
}
