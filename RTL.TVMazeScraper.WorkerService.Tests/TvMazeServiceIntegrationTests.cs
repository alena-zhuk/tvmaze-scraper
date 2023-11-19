using Microsoft.Extensions.DependencyInjection;

namespace RTL.TVMazeScraper.WorkerService.Tests
{
    public class TvMazeServiceIntegrationTests
    {

        ITVMazeService tvMazeService;

        [SetUp]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient<ITVMazeService, TvMazeService>();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();


            tvMazeService = serviceProvider.GetService<ITVMazeService>();
        }

        [Test]
        public async Task CanGetUpdatedShowsOfAllTimes()
        {
            var showsAllTimes = await tvMazeService.GetUpdatedShows(null, CancellationToken.None);
            CollectionAssert.IsNotEmpty(showsAllTimes);
        }

        [Test]
        public async Task CanGetShowsUpdatedLastMonth()
        {
            var showsUpdatedLastMonth = await tvMazeService.GetUpdatedShows(DateTimeOffset.Now.AddMonths(-1), CancellationToken.None);
            CollectionAssert.IsNotEmpty(showsUpdatedLastMonth); // bad test, there is a chance that nothing was updated
        }

        [Test]
        public async Task CanGetGetAShowWithCast()
        {
            var show = await tvMazeService.GetShowWithCast(12, CancellationToken.None);
            Assert.IsNotNull(show);
            Assert.That(show.Name, Is.EqualTo("Lost Girl"));
            Assert.That(show.Cast, Is.Not.Empty);
        }
    }
}