using LifeTimelineApi.Data;
using LifeTimelineApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DBContext _dbContext;

    public AuthController(DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.Email == request.Email);

        if (userExists)
        {
            return BadRequest(new { message = "Email already exists." });
        }

        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, JwtService jwtService)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = jwtService.Generate(user);
        // Here you would typically generate a JWT token and return it to the client
        return Ok(new { message = "Login successful.", token });
    }
}
