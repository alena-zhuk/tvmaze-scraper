using RTL.TVMazeScraper.Storage;
using RTL.TVMazeScraper.Storage.Local;

namespace RTL.TVMazeScraper.WorkerService;

public class ShowScraperWorker : BackgroundService
{
    private readonly ILogger<ShowScraperWorker> _logger;
    private readonly ITVMazeService _tvMazeService;

    private readonly IStorage _storage;

    // todo: store in the db
    private DateTimeOffset? _lastExecutedOn = null;

    public ShowScraperWorker(ILogger<ShowScraperWorker> logger, ITVMazeService tvMazeService, IStorage storage)
    {
        _logger = logger;
        _tvMazeService = tvMazeService;
        _storage = storage;
    }
      
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var executionStartedOn = DateTimeOffset.Now;
            _logger.LogInformation("ShowScraperWorker running at: {time}", DateTimeOffset.Now);
            await DoWork(stoppingToken);
            _logger.LogInformation("ShowScraperWorker completed at: {time}. Total execution time: {time}", DateTimeOffset.Now, DateTimeOffset.Now - executionStartedOn);
            await Task.Delay(executionStartedOn.AddDays(1) - DateTimeOffset.Now, stoppingToken); // run every 24h later to get fresh updates 
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        try
        {
            // get ids of shows which were updated since the last run, all shows if it's the first run
            var updatedShows = await _tvMazeService.GetUpdatedShows(_lastExecutedOn, cancellationToken);
            //todo: introduce a queue and queue up all updated shows. Introduce a separate worker for handling show info

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 3,
                CancellationToken = cancellationToken
            };

            // for each newly added/updated show query details and cast
            await Parallel.ForEachAsync(updatedShows, parallelOptions, async (showId, token) =>
            {
                try
                {
                    // get details & cast
                    var show = await _tvMazeService.GetShowWithCast(showId, token);
                    if (show != null)
                    {
                        // store in the storage
                        await _storage.UpsertAsync(show, cancellationToken);
                    }
                }
                catch (StorageException se) when(se.IsNonTransient)
                {
                    throw;
                }
                catch (Exception e)
                {
                    //swallow exceptions and try to get other shows
                    // todo: ideally when the show processing logic is moved to a separate worker and guarded by a queue, failed to get shows must be re-queued
                    _logger.LogError(e, $"Failed to get show info. Id: {showId}");
                }

            });

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
        _storage.Dispose();
    }
}