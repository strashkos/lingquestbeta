using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinguaQuest.Services;

public class MongoInitializer : IHostedService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoInitializer> _logger;

    public MongoInitializer(MongoDbContext context, ILogger<MongoInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting MongoDB initialization (indexes + seed)...");
            await _context.EnsureIndexesAsync();
            await _context.SeedDataAsync();
            _logger.LogInformation("MongoDB initialization completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MongoDB initialization failed. The application can still run but DB features may be degraded.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
