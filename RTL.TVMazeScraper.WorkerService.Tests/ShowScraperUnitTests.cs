using Microsoft.Extensions.Logging;
using Moq;
using RTL.TVMazeScraper.Models;
using RTL.TVMazeScraper.Storage;

namespace RTL.TVMazeScraper.WorkerService.Tests
{
    internal class ShowScraperUnitTests
    {

        [Test]
        public async Task Run_WhenShowIsUpdatedNewMetadataIsRequestedAndStored()
        {
            // Arrange
            var updatedShowsTestData = new List<long>() { 1, 3, 8 };

            var tvMazeServiceMock = new Mock<ITVMazeService>();
            tvMazeServiceMock.Setup(x => x.GetUpdatedShows(It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedShowsTestData);
            tvMazeServiceMock.Setup(x => x.GetShowWithCast(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((long id, CancellationToken token) => new Show { Id = id });

            var storageMock = new Mock<IStorage>();
            var showScraper = new ShowScraper(new Mock<ILogger<ShowScraper>>().Object, tvMazeServiceMock.Object, storageMock.Object);

            // Act
            await showScraper.Run(null, CancellationToken.None);

            // Assert
            updatedShowsTestData.ForEach(
                id =>
                {
                    // validate that show details and cast were requested for each updated show
                    tvMazeServiceMock.Verify(x =>
                        x.GetShowWithCast(It.Is<long>(v => v.Equals(id)),
                            It.IsAny<CancellationToken>()));

                    // validate that show details are upsterted to the storage for each updated show
                    storageMock.Verify(x =>
                        x.UpsertAsync(It.Is<object>(v => v.Equals(id)),
                            It.Is<Show>(v => v.Id == id),
                            It.IsAny<CancellationToken>()));
                });
        }
    }
}
