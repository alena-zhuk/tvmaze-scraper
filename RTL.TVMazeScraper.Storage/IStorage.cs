namespace RTL.TVMazeScraper.Storage;

public interface IStorage : IDisposable
{
    Task<IEnumerable<T>> Get<T>(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<bool> UpsertAsync<T>(T entity, CancellationToken cancellationToken);
}