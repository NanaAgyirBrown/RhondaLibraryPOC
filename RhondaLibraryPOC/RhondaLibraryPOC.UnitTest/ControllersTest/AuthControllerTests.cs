using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RhondaLibraryPOC.Application.Interfaces.AuthService;
using RhondaLibraryPOC.Presentation.Controllers;
using Shouldly;

namespace RhondaLibraryPOC.UnitTest.ControllersTest;

public class AuthControllerTests
{
    [Fact]
    public void Login_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("rhondalibrarysecretinplaintext1234567890098");
        configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("testIssuer");
        configurationMock.Setup(x => x["Jwt:Audience"]).Returns("testAudience");
        configurationMock.Setup(x => x["Jwt:ExpirationInMinutes"]).Returns("60");

        var loggerMock = new Mock<ILogger<AuthController>>();
        var authService = new Mock<IUserService>();

        var authController = new AuthController(configurationMock.Object, loggerMock.Object, authService.Object);

        var invalidModel = new LoginModel
        {
            Username = "",
            Password = ""
        };

        // Act
        var result = authController.Login(invalidModel) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Invalid client request", result.Value);
    }

    [Fact]
    public void Login_InvalidUser_ReturnsUnauthorizedResult()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthController>>();
        var userServiceMock = new Mock<IUserService>();

        var authController = new AuthController(configurationMock.Object, loggerMock.Object, userServiceMock.Object);

        var model = new LoginModel
        {
            Username = "invalidUser",
            Password = "invalidPassword"
        };

        configurationMock.Setup(config => config["UserSignIn:invalidUser:Username"]).Returns("invalidUser");
        configurationMock.Setup(config => config["UserSignIn:invalidUser:Password"]).Returns("wrongPassword");

        userServiceMock.Setup(service => service.GetRoleByUsername("invalidUser")).Returns("UserRole");

        // Act
        var result = authController.Login(model) as UnauthorizedResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(401);
    }


    [Fact]
    public void Login_ValidUser_ReturnsOkObjectResultWithToken()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthController>>();
        var userServiceMock = new Mock<IUserService>();

        var authController = new AuthController(configurationMock.Object, loggerMock.Object, userServiceMock.Object);

        var model = new LoginModel
        {
            Username = "validUser",
            Password = "validPassword"
        };

        configurationMock.Setup(config => config["Jwt:SecretKey"]).Returns("rhondalibrarysecretinplaintext1234567890098");
        configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("testIssuer");
        configurationMock.Setup(config => config["Jwt:Audience"]).Returns("testAudience");
        configurationMock.Setup(config => config["Jwt:ExpirationInMinutes"]).Returns("60");
        configurationMock.Setup(config => config["UserSignIn:validUser:Username"]).Returns("validUser");
        configurationMock.Setup(config => config["UserSignIn:validUser:Password"]).Returns("validPassword");

        userServiceMock.Setup(service => service.GetRoleByUsername("validUser")).Returns("UserRole");


        (string username, string role) credentials = (model.Username.ToString(), "UserRole");

        userServiceMock.Setup(service => service.GenerateJwtToken(credentials)).Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");

        // Act
        var result = authController.Login(model) as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var token = result.Value.ToString().ShouldBeAssignableTo<string>();
        token.ShouldBe("{ Token = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c }");
    }
}
