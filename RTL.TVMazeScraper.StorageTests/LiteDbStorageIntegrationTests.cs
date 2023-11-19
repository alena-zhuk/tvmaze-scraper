using Microsoft.Extensions.Configuration;
using Moq;
using RTL.TVMazeScraper.Storage.Local;

namespace RTL.TVMazeScraper.StorageTests
{
    public class LiteDbStorageIntegrationTests
    {
        const string testDBFileName = "..\\..\\..\\..\\litedb\\tvmaze-test.db";
        const string testDBConnectionString = $"Filename={testDBFileName};Password=amaze";
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
            // drop the db
            File.Delete(testDBFileName);

        }
        
        [Test]
        public void CanCreate()
        {
            Assert.IsNotNull(_storage);
        }


        [Test]
        public async Task WhenNewEntityIsUpsertedItCanBeRetrieved()
        {
            // Arrange
            var testEntity = new TestEntity { Id = 34, Name = "ewfef" };
            //Act
            var result = await _storage.UpsertAsync(testEntity.Id, testEntity, CancellationToken.None);
            var readResult = await _storage.Get<TestEntity>(0, 1000, CancellationToken.None);

            //Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(readResult);

            var readTestEntity = readResult.SingleOrDefault(x => x.Id == testEntity.Id && x.Name == testEntity.Name);
            Assert.IsNotNull(readTestEntity);
        }

        [Test]
        public async Task WhenUpsertingEntityWithTheSameIdItShouldUpdate()
        {
            // Arrange
            var testEntity = new TestEntity { Id = 34, Name = "ewfef" };
            var testEntityUpdated = new TestEntity { Id = 34, Name = "new value" };
            //Act
            var resultFirst = await _storage.UpsertAsync(testEntity.Id, testEntity, CancellationToken.None);
            var resultSecond = await _storage.UpsertAsync(testEntityUpdated.Id, testEntityUpdated, CancellationToken.None);
            var readResult = await _storage.Get<TestEntity>(0, 1000, CancellationToken.None);

            //Assert
            Assert.IsTrue(resultFirst);
            Assert.IsFalse(resultSecond); // true = inserted; false = updated; pfft..
            Assert.IsNotNull(readResult);
            Assert.That(readResult.Count(x => x.Id == testEntity.Id), Is.EqualTo(1));
            Assert.That(readResult.Single(x => x.Id == testEntity.Id).Name, Is.EqualTo(testEntityUpdated.Name));

        }
    }

    class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}