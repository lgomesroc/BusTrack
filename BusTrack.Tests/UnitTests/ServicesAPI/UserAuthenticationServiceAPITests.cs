using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.Tests.UnitTests.ServicesAPI
{
    public class UserAuthenticationServiceAPITests
    {
        private readonly Mock<IUserRepositoryDB> _userRepositoryMock;
        private readonly UserAuthenticationServiceAPI _service;

        public UserAuthenticationServiceAPITests()
        {
            _userRepositoryMock =
                new Mock<IUserRepositoryDB>();

            _service =
                new UserAuthenticationServiceAPI(
                    _userRepositoryMock.Object);
        }

        [Fact]
        public void Authenticate_UserDoesNotExist_ReturnsNull()
        {
            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmail("missing@example.com"))
                .Returns((UserDB?)null);

            var result =
                _service.Authenticate(
                    "missing@example.com",
                    "password");

            Assert.Null(result);

            _userRepositoryMock.Verify(
                repository =>
                    repository.GetByEmail("missing@example.com"),
                Times.Once);
        }

        [Fact]
        public void Authenticate_UserHasNoPassword_ReturnsNull()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = ""
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmail(user.Email))
                .Returns(user);

            var result =
                _service.Authenticate(
                    user.Email,
                    "password");

            Assert.Null(result);
        }

        [Fact]
        public void Authenticate_InvalidPassword_ReturnsNull()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword(
                    "correct-password")
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmail(user.Email))
                .Returns(user);

            var result =
                _service.Authenticate(
                    user.Email,
                    "wrong-password");

            Assert.Null(result);
        }

        [Fact]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword(
                    "correct-password")
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmail(user.Email))
                .Returns(user);

            var result =
                _service.Authenticate(
                    user.Email,
                    "correct-password");

            Assert.NotNull(result);
            Assert.Same(user, result);
        }

        [Fact]
        public void Create_WithoutPassword_ThrowsArgumentException()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = ""
            };

            var exception =
                Assert.Throws<ArgumentException>(
                    () => _service.Create(user));

            Assert.Contains(
                "senha",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            _userRepositoryMock.Verify(
                repository =>
                    repository.Create(It.IsAny<UserDB>()),
                Times.Never);
        }

        [Fact]
        public void Create_WithPassword_HashesPasswordAndCreatesUser()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = "plain-password"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.Create(It.IsAny<UserDB>()))
                .Returns(user);

            var result =
                _service.Create(user);

            Assert.IsType<OkResult>(result);

            Assert.NotEqual(
                "plain-password",
                user.Password);

            Assert.True(
                BCrypt.Net.BCrypt.Verify(
                    "plain-password",
                    user.Password));

            _userRepositoryMock.Verify(
                repository =>
                    repository.Create(user),
                Times.Once);
        }

        [Fact]
        public void Read_DelegatesToRepository()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.Read("user-1"))
                .Returns(user);

            var result =
                _service.Read("user-1");

            Assert.Same(user, result);

            _userRepositoryMock.Verify(
                repository =>
                    repository.Read("user-1"),
                Times.Once);
        }

        [Fact]
        public void Read_WhenUserDoesNotExist_ReturnsNull()
        {
            _userRepositoryMock
                .Setup(repository =>
                    repository.Read("missing"))
                .Returns((UserDB?)null);

            var result =
                _service.Read("missing");

            Assert.Null(result);
        }

        [Fact]
        public void Update_WithoutPassword_ThrowsArgumentException()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = ""
            };

            var exception =
                Assert.Throws<ArgumentException>(
                    () => _service.Update(
                        "user-1",
                        user));

            Assert.Contains(
                "senha",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            _userRepositoryMock.Verify(
                repository =>
                    repository.Update(
                        It.IsAny<string>(),
                        It.IsAny<UserDB>()),
                Times.Never);
        }

        [Fact]
        public void Update_WithPasswordAndExistingUser_ReturnsOkAndHashesPassword()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = "new-password"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.Update(
                        "user-1",
                        user))
                .Returns(true);

            var result =
                _service.Update(
                    "user-1",
                    user);

            Assert.IsType<OkResult>(result);

            Assert.NotEqual(
                "new-password",
                user.Password);

            Assert.True(
                BCrypt.Net.BCrypt.Verify(
                    "new-password",
                    user.Password));

            _userRepositoryMock.Verify(
                repository =>
                    repository.Update(
                        "user-1",
                        user),
                Times.Once);
        }

        [Fact]
        public void Update_WhenRepositoryReturnsFalse_ReturnsNotFound()
        {
            var user = new UserDB
            {
                Id = "user-1",
                Email = "user@example.com",
                Password = "new-password"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.Update(
                        "user-1",
                        user))
                .Returns(false);

            var result =
                _service.Update(
                    "user-1",
                    user);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_WhenRepositoryReturnsTrue_ReturnsOk()
        {
            _userRepositoryMock
                .Setup(repository =>
                    repository.Delete("user-1"))
                .Returns(true);

            var result =
                _service.Delete("user-1");

            Assert.IsType<OkResult>(result);

            _userRepositoryMock.Verify(
                repository =>
                    repository.Delete("user-1"),
                Times.Once);
        }

        [Fact]
        public void Delete_WhenRepositoryReturnsFalse_ReturnsNotFound()
        {
            _userRepositoryMock
                .Setup(repository =>
                    repository.Delete("user-1"))
                .Returns(false);

            var result =
                _service.Delete("user-1");

            Assert.IsType<NotFoundResult>(result);

            _userRepositoryMock.Verify(
                repository =>
                    repository.Delete("user-1"),
                Times.Once);
        }
    }
}
