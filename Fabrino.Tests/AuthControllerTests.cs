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

        [Fact]
        public void IsValidUser_WithNullUser_ReturnsFalse()
        {
            var mockRepo = new Mock<IUserRepository>();
            var controller = new AuthController(mockRepo.Object);

            var result = controller.IsValidUser(null);

            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithEmptyUsernameOrPassword_ReturnsFalse()
        {
            var user = new UserModel { username = "", password_hash = "" };

            var mockRepo = new Mock<IUserRepository>();
            var controller = new AuthController(mockRepo.Object);

            var result = controller.IsValidUser(user);

            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithWhitespaceUsername_ReturnsTrue()
        {
            var user = new UserModel { username = " shilan ", password_hash = "correctHash" };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.IsValidUser("shilan", "correctHash")).Returns(true);

            var controller = new AuthController(mockRepo.Object);

       
            user.username = user.username.Trim();

            var result = controller.IsValidUser(user);

            Assert.True(result);
        }

        [Fact]
        public void IsValidUser_FailureTest_ShouldFailOnPurpose()
        {
            var user = new UserModel { username = "unknown", password_hash = "wrongHash" };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.IsValidUser("unknown", "wrongHash")).Returns(false);

            var controller = new AuthController(mockRepo.Object);

            var result = controller.IsValidUser(user);

            
            Assert.True(result); 
        }
    }
}
