using System.Net;
using Polly;
using RTL.TVMazeScraper.Storage.Local;
using RTL.TVMazeScraper.WorkerService;

IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerService>();
        services.AddHttpClient<ITVMazeService, TvMazeService>().AddPolicyHandler(GetRetryPolicy());
        services.AddLocalStorage();
        services.AddTransient<IShowScraper, ShowScraper>();
    })
    .Build();

host.Run();
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return Policy.Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => r.StatusCode == (HttpStatusCode)429) // RetryAfter
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: (retryCount, response, context) =>
            {
                // respect retry-after otherwise exponential back-off
                if (response.Result?.Headers?.RetryAfter != null)
                {
                    var retryAfter = response.Result.Headers.RetryAfter.Delta ??
                                     response.Result.Headers.RetryAfter.Date - DateTimeOffset.Now;
                    if (retryAfter != null)
                        return retryAfter.Value;
                }
                return TimeSpan.FromSeconds(Math.Pow(2, retryCount)); // exponential back-off
            },
            onRetryAsync: async (response, timespan, retryCount, context) =>
            {
                // todo: maybe some logging?
            });
}
