using Microsoft.Extensions.DependencyInjection;

namespace RTL.TVMazeScraper.Storage.Local
{
    public static class LocalStorageServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalStorage(
            this IServiceCollection services)
        {
            return services.AddSingleton<IStorage, LiteDbStorage>();
        }
    }
}
