using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RhondaLibraryPOC.Presentation.Controllers;

namespace RhondaLibraryPOC.UnitTest.ControllersTest;

public class AuthControllerTests
{
    [Fact]
    public void Login_InvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("rhondalibrarysecretinplaintext1234567890098");
        configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("yourIssuer");
        configurationMock.Setup(x => x["Jwt:Audience"]).Returns("yourAudience");
        configurationMock.Setup(x => x["Jwt:ExpirationInMinutes"]).Returns("60");

        var loggerMock = new Mock<ILogger<AuthController>>();

        var authController = new AuthController(configurationMock.Object, loggerMock.Object);

        var invalidModel = new LoginModel
        {
            Username = "invalidUsername",
            Password = "invalidPassword"
        };

        // Act
        var result = authController.Login(invalidModel) as UnauthorizedResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void Login_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("rhondalibrarysecretinplaintext1234567890098");
        configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("yourIssuer");
        configurationMock.Setup(x => x["Jwt:Audience"]).Returns("yourAudience");
        configurationMock.Setup(x => x["Jwt:ExpirationInMinutes"]).Returns("60");

        var loggerMock = new Mock<ILogger<AuthController>>();

        var authController = new AuthController(configurationMock.Object, loggerMock.Object);

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
    public void Login_ValidUser_ReturnsOkWithToken()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("rhondalibrarysecretinplaintext1234567890098");
        configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("yourIssuer");
        configurationMock.Setup(x => x["Jwt:Audience"]).Returns("yourAudience");
        configurationMock.Setup(x => x["Jwt:ExpirationInMinutes"]).Returns("60");

        var loggerMock = new Mock<ILogger<AuthController>>();

        var authController = new AuthController(configurationMock.Object, loggerMock.Object);

        var validModel = new LoginModel
        {
            Username = "validUsername",
            Password = "validPassword"
        };

        // Act
        var result = authController.Login(validModel) as OkResult;

        // Assert
        Assert.Null(result);

    }
}
