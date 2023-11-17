namespace RTL.TVMazeScraper.Storage;

public interface IStorage
{
    Task<IEnumerable<T>> Get<T>(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task Upsert<T>(T entity);
}