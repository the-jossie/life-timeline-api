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

        var accessToken = _jwtService.Generate(user);


        var refreshToken =
            _refreshTokenService.Generate();


        var refreshTokenHash =
            _refreshTokenService.Hash(refreshToken);


        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            TokenHash = refreshTokenHash,

            CreatedAt = DateTime.UtcNow,

            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };


        _dbContext.RefreshTokens.Add(refreshTokenEntity);

        await _dbContext.SaveChangesAsync();


        return Ok(new
        LoginResponse
        {
            Message = "Login successful.",
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
      RefreshTokenRequest request,
      JwtService jwtService,
      RefreshTokenService refreshTokenService)
    {
        var tokenHash =
            refreshTokenService.Hash(request.RefreshToken);

        var storedToken =
            await _dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash
            );

        if (storedToken == null ||
            !storedToken.IsActive)
        {
            return Unauthorized(new
            {
                message = "Invalid refresh token"
            });
        }

        // rotate token
        storedToken.RevokedAt =
            DateTime.UtcNow;
        var newRefreshToken =
            refreshTokenService.Generate();
        var newHash =
            refreshTokenService.Hash(newRefreshToken);

        _dbContext.RefreshTokens.Add(
            new RefreshToken
            {
                Id = Guid.NewGuid(),

                UserId = storedToken.UserId,

                TokenHash = newHash,

                CreatedAt = DateTime.UtcNow,

                ExpiresAt =
                    DateTime.UtcNow.AddDays(30)
            });

        var newAccessToken =
            jwtService.Generate(storedToken.User);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            accessToken = newAccessToken,

            refreshToken = newRefreshToken
        });
    }
}
