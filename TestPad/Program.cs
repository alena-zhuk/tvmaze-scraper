using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly.Extensions.Http;
using Polly;
using RTL.TVMazeScraper.WorkerService;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    // todo: handle RetryAfter header
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
            retryAttempt)));
}


//setup DI
var serviceCollection = new ServiceCollection();
serviceCollection.AddHttpClient<ITVMazeService, TvMazeService>().AddPolicyHandler(GetRetryPolicy());
serviceCollection.AddLogging();
var serviceProvider = serviceCollection.BuildServiceProvider();


var tvMazeService = serviceProvider.GetService<ITVMazeService>();
var showsAllTimes = await tvMazeService.GetUpdatedShows(null, CancellationToken.None);
var showsUpdatedLastDay = await tvMazeService.GetUpdatedShows(DateTimeOffset.Now.AddDays(-1), CancellationToken.None);
var show = await tvMazeService.GetShowWithCast(1, CancellationToken.None);
Console.WriteLine($"{show.Name} / {show.Embedded?.Cast.Count}");
var show12 = await tvMazeService.GetShowWithCast(12, CancellationToken.None);
Console.WriteLine($"{show12.Name} / {show12.Embedded?.Cast.Count}");
