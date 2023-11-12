using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RhondaLibraryPOC.Presentation.Controllers;

public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost, Route("login")]
    public IActionResult Login(LoginModel model)
    {
        if (model == null)
        {
            return BadRequest("Invalid client request");
        }

        if (IsValidUser(model))
        {
            var tokenString = GenerateJwtToken();
            return Ok(new { Token = tokenString });
        }
        else
        {
            return Unauthorized();
        }
    }

    private bool IsValidUser(LoginModel model)
    {
        // Replace this logic with your actual user validation logic.
        return model.Username == "johndoe" && model.Password == "admin@1234";
    }

    private string GenerateJwtToken()
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: new List<Claim>
            {
                new Claim(ClaimTypes.Name, "johndoe"),
                new Claim(ClaimTypes.Role, "Amin"),
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
