namespace RTL.TVMazeScraper.Storage.Local;

public class StorageException : Exception
{
    public bool IsNonTransient { get; set; }
    public StorageException(bool isNonTransient)
    {
        IsNonTransient = isNonTransient;
    }

    public StorageException(string message, bool isNonTransient) : base(message)
    {
        IsNonTransient = isNonTransient;
    }

    public StorageException(string message, Exception inner, bool isNonTransient) : base(message, inner)
    {
        IsNonTransient = isNonTransient;
    }
}