using System.Security.Claims;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;

namespace BusTrack.BusTrack.Tests.UnitTests.ControllersAPIUnitTests
{
    public class AuthenticationControllerAPIUnitTests
    {
        private readonly Mock<IUserAuthenticationServiceAPI> _userServiceMock;
        private readonly Mock<IJwtTokenServiceAPI> _jwtTokenServiceMock;
        private readonly Mock<IUserRepositoryDB> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMongoDatabase> _databaseMock;

        public AuthenticationControllerAPIUnitTests()
        {
            _userServiceMock =
                new Mock<IUserAuthenticationServiceAPI>();

            _jwtTokenServiceMock =
                new Mock<IJwtTokenServiceAPI>();

            _userRepositoryMock =
                new Mock<IUserRepositoryDB>();

            _configurationMock =
                new Mock<IConfiguration>();

            _databaseMock =
                new Mock<IMongoDatabase>();
        }

        private AuthenticationControllerAPI CreateController(
            string? currentUserId = null)
        {
            var controller =
                new AuthenticationControllerAPI(
                    _userServiceMock.Object,
                    _jwtTokenServiceMock.Object,
                    _userRepositoryMock.Object,
                    _configurationMock.Object,
                    _databaseMock.Object);

            if (currentUserId is not null)
            {
                controller.ControllerContext =
                    new ControllerContext
                    {
                        HttpContext =
                            new DefaultHttpContext
                            {
                                User =
                                    new ClaimsPrincipal(
                                        new ClaimsIdentity(
                                            new[]
                                            {
                                                new Claim(
                                                    ClaimTypes.NameIdentifier,
                                                    currentUserId)
                                            },
                                            "TestAuthentication"))
                            }
                    };
            }

            return controller;
        }

        [Fact]
        public void Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            _userServiceMock
                .Setup(service =>
                    service.Authenticate(
                        "user@example.com",
                        "wrong-password"))
                .Returns((UserDB?)null);

            var controller =
                CreateController();

            var request =
                new LoginRequestDTOAPI
                {
                    Email = "user@example.com",
                    Password = "wrong-password"
                };

            var result =
                controller.Login(request);

            var unauthorized =
                Assert.IsType<UnauthorizedObjectResult>(result);

            Assert.Equal(
                401,
                unauthorized.StatusCode);

            _jwtTokenServiceMock.Verify(
                service =>
                    service.CreateToken(
                        It.IsAny<UserDB>()),
                Times.Never);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsAuthenticationResponse()
        {
            var user =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Usuário Teste",
                    Email = "user@example.com",
                    Role = "USER"
                };

            var expiresAt =
                DateTime.UtcNow.AddMinutes(30);

            _userServiceMock
                .Setup(service =>
                    service.Authenticate(
                        "user@example.com",
                        "correct-password"))
                .Returns(user);

            _jwtTokenServiceMock
                .Setup(service =>
                    service.CreateToken(user))
                .Returns(
                    ("test-access-token", expiresAt));

            var controller =
                CreateController();

            var request =
                new LoginRequestDTOAPI
                {
                    Email = "user@example.com",
                    Password = "correct-password"
                };

            var result =
                controller.Login(request);

            var ok =
                Assert.IsType<OkObjectResult>(result);

            var response =
                Assert.IsType<AuthenticationResponseDTOAPI>(
                    ok.Value);

            Assert.True(response.Success);
            Assert.Equal(
                "test-access-token",
                response.AccessToken);

            Assert.Equal(
                expiresAt,
                response.ExpiresAtUtc);

            Assert.Equal(
                "Bearer",
                response.TokenType);

            Assert.Contains(
                "Seja bem-vindo",
                response.WelcomeMessage);

            _userServiceMock.Verify(
                service =>
                    service.Authenticate(
                        "user@example.com",
                        "correct-password"),
                Times.Once);

            _jwtTokenServiceMock.Verify(
                service =>
                    service.CreateToken(user),
                Times.Once);
        }

        [Fact]
        public void Login_WithNullEmailAndPassword_PassesEmptyStringsToService()
        {
            _userServiceMock
                .Setup(service =>
                    service.Authenticate(
                        string.Empty,
                        string.Empty))
                .Returns((UserDB?)null);

            var controller =
                CreateController();

            var request =
                new LoginRequestDTOAPI
                {
                    Email = null,
                    Password = null
                };

            var result =
                controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(
                result);

            _userServiceMock.Verify(
                service =>
                    service.Authenticate(
                        string.Empty,
                        string.Empty),
                Times.Once);
        }

        [Fact]
        public void Create_AlwaysForcesUserRole()
        {
            var user =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Novo Usuário",
                    Email = "new@example.com",
                    Password = "password",
                    Role = "ADMIN"
                };

            _userServiceMock
                .Setup(service =>
                    service.Create(user))
                .Returns(new OkResult());

            var controller =
                CreateController();

            var result =
                controller.Create(user);

            var ok =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                200,
                ok.StatusCode);

            Assert.Equal(
                "USER",
                user.Role);

            _userServiceMock.Verify(
                service =>
                    service.Create(user),
                Times.Once);
        }

        [Fact]
        public void Read_CurrentUser_ReturnsUserData()
        {
            var user =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Usuário Teste",
                    CPF = "12345678900",
                    Email = "user@example.com",
                    Login = "user"
                };

            _userServiceMock
                .Setup(service =>
                    service.Read("user-1"))
                .Returns(user);

            var controller =
                CreateController("user-1");

            var result =
                controller.Read("user-1");

            var ok =
                Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(ok.Value);

            var valueType =
                ok.Value!.GetType();

            var id =
                valueType.GetProperty("Id")!
                    .GetValue(ok.Value);

            var fullName =
                valueType.GetProperty("FullName")!
                    .GetValue(ok.Value);

            var cpf =
                valueType.GetProperty("CPF")!
                    .GetValue(ok.Value);

            var email =
                valueType.GetProperty("Email")!
                    .GetValue(ok.Value);

            var login =
                valueType.GetProperty("Login")!
                    .GetValue(ok.Value);

            Assert.Equal("user-1", id);
            Assert.Equal("Usuário Teste", fullName);
            Assert.Equal("12345678900", cpf);
            Assert.Equal("user@example.com", email);
            Assert.Equal("user", login);
        }

        [Fact]
        public void Read_DifferentUser_ReturnsForbidden()
        {
            var controller =
                CreateController("user-1");

            var result =
                controller.Read("user-2");

            Assert.IsType<ForbidResult>(result);

            _userServiceMock.Verify(
                service =>
                    service.Read(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void Read_CurrentUser_WhenUserDoesNotExist_ReturnsNotFound()
        {
            _userServiceMock
                .Setup(service =>
                    service.Read("user-1"))
                .Returns((UserDB?)null);

            var controller =
                CreateController("user-1");

            var result =
                controller.Read("user-1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_DifferentUser_ReturnsForbidden()
        {
            var user =
                new UserDB
                {
                    Id = "user-2",
                    FullName = "Outro Usuário",
                    Password = "password"
                };

            var controller =
                CreateController("user-1");

            var result =
                controller.Update(
                    "user-2",
                    user);

            Assert.IsType<ForbidResult>(result);

            _userServiceMock.Verify(
                service =>
                    service.Read(It.IsAny<string>()),
                Times.Never);

            _userServiceMock.Verify(
                service =>
                    service.Update(
                        It.IsAny<string>(),
                        It.IsAny<UserDB>()),
                Times.Never);
        }

        [Fact]
        public void Update_CurrentUser_WhenUserDoesNotExist_ReturnsNotFound()
        {
            var user =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Usuário Teste",
                    Password = "password"
                };

            _userServiceMock
                .Setup(service =>
                    service.Read("user-1"))
                .Returns((UserDB?)null);

            var controller =
                CreateController("user-1");

            var result =
                controller.Update(
                    "user-1",
                    user);

            Assert.IsType<NotFoundResult>(result);

            _userServiceMock.Verify(
                service =>
                    service.Update(
                        It.IsAny<string>(),
                        It.IsAny<UserDB>()),
                Times.Never);
        }

        [Fact]
        public void Update_CurrentUser_PreservesExistingRole()
        {
            var existingUser =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Usuário Antigo",
                    Email = "old@example.com",
                    Role = "USER"
                };

            var user =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Usuário Atualizado",
                    Email = "new@example.com",
                    Password = "new-password",
                    Role = "ADMIN"
                };

            _userServiceMock
                .Setup(service =>
                    service.Read("user-1"))
                .Returns(existingUser);

            _userServiceMock
                .Setup(service =>
                    service.Update(
                        "user-1",
                        user))
                .Returns(new OkResult());

            var controller =
                CreateController("user-1");

            var result =
                controller.Update(
                    "user-1",
                    user);

            Assert.IsType<OkResult>(result);

            Assert.Equal(
                "user-1",
                user.Id);

            Assert.Equal(
                "USER",
                user.Role);

            _userServiceMock.Verify(
                service =>
                    service.Read("user-1"),
                Times.Once);

            _userServiceMock.Verify(
                service =>
                    service.Update(
                        "user-1",
                        user),
                Times.Once);
        }

        [Fact]
        public void Update_CurrentUser_WhenServiceReturnsNotFound_ReturnsNotFound()
        {
            var existingUser =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Usuário Teste",
                    Role = "USER"
                };

            var user =
                new UserDB
                {
                    Id = "user-1",
                    FullName = "Usuário Atualizado",
                    Password = "password"
                };

            _userServiceMock
                .Setup(service =>
                    service.Read("user-1"))
                .Returns(existingUser);

            _userServiceMock
                .Setup(service =>
                    service.Update(
                        "user-1",
                        user))
                .Returns(new NotFoundResult());

            var controller =
                CreateController("user-1");

            var result =
                controller.Update(
                    "user-1",
                    user);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_DifferentUser_ReturnsForbidden()
        {
            var controller =
                CreateController("user-1");

            var result =
                controller.Delete("user-2");

            Assert.IsType<ForbidResult>(result);

            _userServiceMock.Verify(
                service =>
                    service.Delete(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void Delete_CurrentUser_ReturnsServiceResult()
        {
            _userServiceMock
                .Setup(service =>
                    service.Delete("user-1"))
                .Returns(new OkResult());

            var controller =
                CreateController("user-1");

            var result =
                controller.Delete("user-1");

            Assert.IsType<OkResult>(result);

            _userServiceMock.Verify(
                service =>
                    service.Delete("user-1"),
                Times.Once);
        }

        [Fact]
        public void Delete_CurrentUser_WhenServiceReturnsNotFound_ReturnsNotFound()
        {
            _userServiceMock
                .Setup(service =>
                    service.Delete("user-1"))
                .Returns(new NotFoundResult());

            var controller =
                CreateController("user-1");

            var result =
                controller.Delete("user-1");

            Assert.IsType<NotFoundResult>(result);

            _userServiceMock.Verify(
                service =>
                    service.Delete("user-1"),
                Times.Once);
        }
    }
}