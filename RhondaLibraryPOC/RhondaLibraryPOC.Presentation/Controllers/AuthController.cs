using Microsoft.AspNetCore.Mvc;
using RhondaLibraryPOC.Application.Interfaces.AuthService;

namespace RhondaLibraryPOC.Presentation.Controllers;

public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger, IUserService userService)
    {
        _configuration = configuration;
        _logger = logger;
        _userService = userService;
    }

    [HttpPost, Route("login")]
    public IActionResult Login(LoginModel model)
    {
        _logger.LogInformation("Logging in user");

        if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            return BadRequest("Invalid client request");

        if (IsValidUser(model))
        {
            string role = _userService.GetRoleByUsername(model.Username);
            var tokenString = _userService.GenerateJwtToken((model.Username, role));
            return Ok(new { Token = tokenString });
        }
        else
            return Unauthorized();
    }

    private bool IsValidUser(LoginModel model)
    {
        _logger.LogInformation("Validating user");

        if (model.Username == _configuration[$"UserSignIn:{model.Username}:Username"] && model.Password == _configuration[$"UserSignIn:{model.Username}:Password"])
            return true;

        return false;
    }
}

public record LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
