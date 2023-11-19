using LiteDB;
using LiteDB.Async;
using Microsoft.Extensions.Configuration;

namespace RTL.TVMazeScraper.Storage.Local;

public class LiteDbStorage : IStorage
{
    private readonly LiteDatabaseAsync _db;

    public LiteDbStorage(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LiteDB")!;
        _db = new LiteDatabaseAsync(connectionString);
    }
    public async Task<IEnumerable<T>> Get<T>(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var collection = _db.GetCollection<T>();
            return await collection.Query().Skip(pageSize * pageNumber).Limit(pageSize).ToListAsync();

        }
        catch (DirectoryNotFoundException ex)
        {
            throw new StorageException("Bad Connection String", ex, true);
        }
        catch (LiteAsyncException ex)
        {
            throw new StorageException(ex.InnerException?.Message ?? ex.Message, ex, true);
        }
        catch (Exception exception)
        {
            throw new StorageException("Unable to execute the query", exception, false);
        }
    }

    public async Task<bool> UpsertAsync<T>(object id, T entity, CancellationToken cancellationToken)
    {
        try
        {
            var collection = _db.GetCollection<T>();

            return await collection.UpsertAsync(new BsonValue(id), entity);

        }
        catch (DirectoryNotFoundException ex)
        {
            throw new StorageException("Bad Connection String", ex, true);
        }
        catch (LiteAsyncException ex)
        {
            throw new StorageException(ex.InnerException?.Message ?? ex.Message, ex, true);
        }
        catch (Exception exception)
        {
            throw new StorageException("Unable to upsert", exception, false);
        }
    }

    private void ReleaseUnmanagedResources()
    {
        // todo: some bug with releasing lite db - throws an exception that CancellationTokenSource was disposed.
        _db.Dispose();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~LiteDbStorage()
    {
        ReleaseUnmanagedResources();
    }
}