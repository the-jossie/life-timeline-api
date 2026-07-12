using LifeTimelineApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LifeTimelineApi.Services.Background;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger;


    public RefreshTokenCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<RefreshTokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupTokens(stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Refresh token cleanup failed"
                );
            }


            // run once every 24 hours
            await Task.Delay(
                TimeSpan.FromDays(1),
                stoppingToken
            );
        }
    }



    private async Task CleanupTokens(
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();


        var db =
            scope.ServiceProvider
            .GetRequiredService<AppDbContext>();


        var expiryDate =
            DateTime.UtcNow.AddDays(-7);



        var tokens =
            await db.RefreshTokens
            .Where(x =>
                x.ExpiresAt < DateTime.UtcNow
                ||
                (
                    x.RevokedAt != null
                    &&
                    x.RevokedAt < expiryDate
                )
            )
            .ToListAsync(cancellationToken);



        if(tokens.Count == 0)
        {
            return;
        }


        db.RefreshTokens.RemoveRange(tokens);


        await db.SaveChangesAsync(
            cancellationToken
        );


        _logger.LogInformation(
            "Deleted {Count} refresh tokens",
            tokens.Count
        );
    }
}
