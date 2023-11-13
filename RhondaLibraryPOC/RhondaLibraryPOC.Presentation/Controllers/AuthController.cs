using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RhondaLibraryPOC.Presentation.Controllers;

public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost, Route("login")]
    public IActionResult Login(LoginModel model)
    {
        _logger.LogInformation("Logging in user");

        if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            return BadRequest("Invalid client request");

        if (IsValidUser(model))
        {
            string role = _configuration[$"UserSignIn:{model.Username}:Role"];
            var tokenString = GenerateJwtToken((model.Username, role));
            return Ok(new { Token = tokenString });
        }
        else
            return Unauthorized();
    }

    private bool IsValidUser(LoginModel model)
    {
        _logger.LogInformation("Validating user");
        // Replace this logic with your actual user validation logic.
        var c = _configuration[$"UserSignIn:{model.Username}:Username"];

        if (model.Username == _configuration[$"UserSignIn:{model.Username}:Username"] && model.Password == _configuration[$"UserSignIn:{model.Username}:Password"])
            return true;

        return false;
    }

    private string GenerateJwtToken((string name, string role) user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Role, user.role),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])).ToString())
            },
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
            signingCredentials: signinCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}

public record LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
