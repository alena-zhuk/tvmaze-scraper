using Microsoft.Extensions.Configuration;
using Moq;
using RTL.TVMazeScraper.Storage.Local;

namespace RTL.TVMazeScraper.StorageTests
{
    public class LiteDbStorageIntegrationTests
    {
        const string testDBConnectionString = "Filename=..\\..\\..\\..\\litedb\\tvmaze-test.db;Connection=shared;Password=amaze";
        Mock<IConfiguration> _mockConfiguration;
        private LiteDbStorage _storage;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "LiteDB")]).Returns(testDBConnectionString);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            _storage = new LiteDbStorage(_mockConfiguration.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _storage.Dispose();
        }

        [Test]
        public void CanCreate()
        {
            Assert.IsNotNull(_storage);
        }


        [Test]
        public async Task WhenNewEntityIsUpsertedItCanBeRetrieved()
        {
            var testEntity = new TestEntity { Id = 34, Name = "ewfef" };
            var result = await _storage.UpsertAsync(testEntity, CancellationToken.None);
            Assert.IsTrue(result);

            var readResult = await _storage.Get<TestEntity>(0, 1000, CancellationToken.None);
            Assert.IsNotNull(readResult);
            var readTestEntity = readResult.SingleOrDefault(x => x.Id == testEntity.Id && x.Name == testEntity.Name);
            Assert.IsNotNull(readTestEntity);
        }
    }

    class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}