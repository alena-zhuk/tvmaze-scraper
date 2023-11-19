using RTL.TVMazeScraper.Storage.Local;

namespace RTL.TVMazeScraper.WorkerService;

public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IShowScraper _showScraper;

    // todo: store in the db
    private DateTimeOffset? _lastExecutedOn = null;

    public WorkerService(ILogger<WorkerService> logger, IShowScraper showScraper)
    {
        _logger = logger;
        _showScraper = showScraper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var executionStartedOn = DateTimeOffset.Now;
            _logger.LogInformation("WorkerService running at: {time}", DateTimeOffset.Now);
            await DoWork(stoppingToken);
            _logger.LogInformation("WorkerService completed at: {time}. Total execution time: {time}", DateTimeOffset.Now, DateTimeOffset.Now - executionStartedOn);
            await Task.Delay(executionStartedOn.AddDays(1) - DateTimeOffset.Now, stoppingToken); // run every 24h later to get fresh updates 
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        try
        {
            await _showScraper.Run(_lastExecutedOn, cancellationToken);

            // update lastExecutedOn after a successful run
            _lastExecutedOn = DateTime.UtcNow;
        }
        catch (StorageException se) when (se.IsNonTransient)
        {
            _logger.LogError(se, "Fatal storage error. Shutting down...");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed attempt to scrape TVMaze API.");
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        (_showScraper as IDisposable)?.Dispose();
    }
}