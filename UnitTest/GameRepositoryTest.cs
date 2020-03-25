using Microsoft.VisualStudio.TestTools.UnitTesting;
using VideoGameAPI.Models;
using VideoGameAPI.Repositories;
using VideoGameAPI.Contexts;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;

namespace UnitTest
{
    [TestClass]
    public class GameRepositoryTest
    {
        private static Fixture _fixture;

        public GameRepositoryTest()
        {
            _fixture = new Fixture();
        }


        [TestMethod]
        public async Task GetGameById_BasicTest()
        {
            // Arrange
            int publisherId = _fixture.Create<int>();

            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                PublisherId = publisherId
            };

            var testPublisher = new PublisherModel()
            {
                PublisherId = publisherId,
                PublisherName = _fixture.Create<string>()
            };

            var gameContextMock = new Mock<GameContext>();
            var publisherContextMock = new Mock<PublisherContext>();
            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { testPublisher });

            gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).ReturnsAsync(testGame);
            publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);

            var gameRepo = new GameRepository(gameContextMock.Object, publisherContextMock.Object);

            // Act
            var gameResult = await gameRepo.GetGameById(testGame.GameId);

            // Assert
            Assert.AreEqual(testGame.GameId, gameResult.Value.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.Value.GameName);
            Assert.AreEqual(testGame.PublisherId, gameResult.Value.PublisherId);
            Assert.AreEqual(testPublisher.PublisherName, gameResult.Value.PublisherName);
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }
    }
}
