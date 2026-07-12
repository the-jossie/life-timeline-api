using LifeTimelineApi.Data;
using LifeTimelineApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly JwtService _jwtService;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthController(AppDbContext dbContext,
        JwtService jwtService,
        RefreshTokenService refreshTokenService)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
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
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

        var token = _jwtService.Generate(user);
        var accessToken = _jwtService.Generate(user);


        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),

            Token = _refreshTokenService.Generate(),

            ExpiresAt = _refreshTokenService.GetExpiryDate(),

            UserId = user.Id
        };


        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync();


        return Ok(new
        LoginResponse
        {
            Message = "Login successful.",
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh")]
public async Task<IActionResult> Refresh(
    RefreshRequest request)
{

    var storedToken =
        await _dbContext.RefreshTokens
        .Include(x => x.User)
        .FirstOrDefaultAsync(
            x => x.Token == request.RefreshToken
        );


    if (storedToken == null)
    {
        return Unauthorized();
    }


    if (storedToken.IsRevoked)
    {
        return Unauthorized();
    }


    if (storedToken.ExpiresAt < DateTime.UtcNow)
    {
        return Unauthorized();
    }


    var newAccessToken =
        _jwtService.Generate(storedToken.User);


    var newRefreshToken = new RefreshToken
    {
        Id = Guid.NewGuid(),

        Token = _refreshTokenService.Generate(),

        ExpiresAt =
            _refreshTokenService.GetExpiryDate(),

        UserId = storedToken.UserId
    };


    storedToken.IsRevoked = true;


    _dbContext.RefreshTokens.Add(newRefreshToken);


    await _dbContext.SaveChangesAsync();


    return Ok(new
    {
        accessToken = newAccessToken,

        refreshToken = newRefreshToken.Token
    });

}
}
