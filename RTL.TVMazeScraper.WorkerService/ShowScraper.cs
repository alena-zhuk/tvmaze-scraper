using RTL.TVMazeScraper.Storage;
using RTL.TVMazeScraper.Storage.Local;

namespace RTL.TVMazeScraper.WorkerService;

public interface IShowScraper
{
    Task Run(DateTimeOffset? lastExecutedOn, CancellationToken cancellationToken);
}
public class ShowScraper : IShowScraper, IDisposable
{
    private readonly ILogger<ShowScraper> _logger;
    private readonly ITVMazeService _tvMazeService;
    private readonly IStorage _storage;

    public ShowScraper(ILogger<ShowScraper> logger, ITVMazeService tvMazeService, IStorage storage)
    {
        _logger = logger;
        _tvMazeService = tvMazeService;
        _storage = storage;
    }

    public async Task Run(DateTimeOffset? lastExecutedOn, CancellationToken cancellationToken)
    {
        // get ids of shows which were updated since the last run, all shows if it's the first run
        var updatedShows = await _tvMazeService.GetUpdatedShows(lastExecutedOn, cancellationToken);
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
                    await _storage.UpsertAsync(show.Id, show, cancellationToken);
                }
            }
            catch (StorageException se) when (se.IsNonTransient)
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

    }

    public void Dispose()
    {
        _storage.Dispose();
    }
}