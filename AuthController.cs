using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AuthDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);

        if (exists)
            return BadRequest("Cet email existe déjà.");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Utilisateur enregistré avec succès",
            user.Id,
            user.FullName,
            user.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User loginUser)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == loginUser.Email &&
            u.Password == loginUser.Password);

        if (user == null)
            return Unauthorized("Email ou mot de passe incorrect.");

        var token = GenerateJwtToken(user.Email);

        return Ok(new { token });
    }

    private string GenerateJwtToken(string email)
    {
        var jwtKey = _configuration["Jwt:Key"];

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "test",
            audience: "test",
            claims: new[] { new Claim(ClaimTypes.Name, email) },
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}