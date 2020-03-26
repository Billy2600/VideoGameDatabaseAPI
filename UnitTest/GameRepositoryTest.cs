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
using System.Threading;

namespace UnitTest
{
    [TestClass]
    public class GameRepositoryTest
    {
        private Fixture _fixture;
        private Mock<GameContext> _gameContextMock;
        private Mock<PublisherContext> _publisherContextMock;
        private Mock<ConsoleContext> _consoleContextMock;

        [TestInitialize]
        public void Initialize()
        {
            _fixture = new Fixture();
            _gameContextMock = new Mock<GameContext>();
            _publisherContextMock = new Mock<PublisherContext>();
            _consoleContextMock = new Mock<ConsoleContext>();
        }

        [TestMethod]
        public async Task GetGameById_BasicTest()
        {
            // Arrange
            var publisherId = _fixture.Create<int>();
            var consoleId = _fixture.Create<int>(); 

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

            var testConsole = new ConsoleModel()
            {
                ConsoleId = consoleId,
                ConsoleName = _fixture.Create<string>()
            };

            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { testPublisher });
            var consoleDbSet = CreateDbSetMock(new List<ConsoleModel>() { testConsole });

            _gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).ReturnsAsync(testGame);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _consoleContextMock.Setup(x => x.Consoles).Returns(consoleDbSet.Object);

            var gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object);

            // Act
            var gameResult = await gameRepo.GetGameById(testGame.GameId);

            // Assert
            Assert.AreEqual(testGame.GameId, gameResult.Value.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.Value.GameName);
            Assert.AreEqual(testGame.PublisherId, gameResult.Value.PublisherId);
            Assert.AreEqual(testPublisher.PublisherName, gameResult.Value.PublisherName);
        }

        [TestMethod]
        public async Task GetGameById_NoPublisher()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                PublisherId = _fixture.Create<int>()
            };

            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { }); // No items on purpose

            _gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).ReturnsAsync(testGame);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);

            var gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object);

            // Act
            var gameResult = await gameRepo.GetGameById(testGame.GameId);

            // Assert
            Assert.AreEqual(testGame.GameId, gameResult.Value.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.Value.GameName);
            Assert.AreEqual(testGame.PublisherId, gameResult.Value.PublisherId);
            Assert.IsNull(testGame.PublisherName);
        }

        [TestMethod]
        public void GetGames_BasicTest()
        {
            // Arrange
            var publisherId = _fixture.Create<int>();
            var consoleId = _fixture.Create<int>();

            _fixture.Customize<GameModel>(x => x
                .With(y => y.PublisherId, publisherId) // Assign all games to testPublisher
                .Without(y => y.PublisherName)
            );
            var testGames = _fixture.CreateMany<GameModel>();

            var testPublisher = new PublisherModel()
            {
                PublisherId = publisherId,
                PublisherName = _fixture.Create<string>()
            };

            var testConsole = new ConsoleModel()
            {
                ConsoleId = consoleId,
                ConsoleName = _fixture.Create<string>()
            };

            var gameDbSet = CreateDbSetMock(testGames);
            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { testPublisher });
            var consoleDbSet = CreateDbSetMock(new List<ConsoleModel>() { testConsole });

            _gameContextMock.Setup(x => x.Games).Returns(gameDbSet.Object);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _consoleContextMock.Setup(x => x.Consoles).Returns(consoleDbSet.Object);

            var gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object);

            // Act
            var gameResults = gameRepo.GetGames();

            // Assert
            Assert.AreEqual(testGames.Count(), gameResults.Count());
            for(int i = 0; i < testGames.Count(); i++)
            {
                var testGame = testGames.ElementAt(i);
                var gameResult = gameResults.ElementAt(i);

                Assert.AreEqual(testGame.GameId, gameResult.GameId);
                Assert.AreEqual(testGame.GameName, gameResult.GameName);
                Assert.AreEqual(testGame.PublisherId, gameResult.PublisherId);
                Assert.AreEqual(testPublisher.PublisherName, gameResult.PublisherName);
            }   
        }

        [TestMethod]
        public async Task UpdateGame_BasicTest()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                PublisherId = _fixture.Create<int>()
            };

            var gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object);

            _gameContextMock.Setup(x => x.SetModified(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

            // Act
            await gameRepo.UpdateGame(testGame);

            // Assert
            _gameContextMock.Verify(x => x.SetModified(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Add_BasicTest()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                PublisherName = _fixture.Create<string>()
            };

            var testPublisher = new PublisherModel()
            {
                PublisherId = _fixture.Create<int>(),
                PublisherName = testGame.PublisherName
            };

            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { testPublisher });

            _gameContextMock.Setup(x => x.Games.Add(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);

            var gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object);

            // Act
            var gameResult = await gameRepo.Add(testGame);

            // Assert
            _gameContextMock.Verify(x => x.Games.Add(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(testGame.GameId, gameResult.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.GameName);
            Assert.AreEqual(testPublisher.PublisherId, gameResult.PublisherId); // Should've automatically mapped Id based on name
            Assert.AreEqual(testPublisher.PublisherName, gameResult.PublisherName);
        }

        [TestMethod]
        public async Task DeleteGame_BasicTest()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>()
            };

            _gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).ReturnsAsync(testGame);
            _gameContextMock.Setup(x => x.Games.Remove(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

            var gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object);

            // Act
            var deleteResult = await gameRepo.DeleteGame(testGame.GameId);

            // Assert
            _gameContextMock.Verify(x => x.Games.FindAsync(It.IsAny<int>()), Times.Once);
            _gameContextMock.Verify(x => x.Games.Remove(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(testGame.GameId, deleteResult.GameId);
        }

        [TestMethod]
        public async Task DeleteGame_NoGameToDelete()
        {
            // Arrange
            var invalidGameId = _fixture.Create<int>();

            _gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).Returns(null);
            _gameContextMock.Setup(x => x.Games.Remove(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

            var gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object);

            // Act
            var deleteResult = await gameRepo.DeleteGame(invalidGameId);

            // Assert
            _gameContextMock.Verify(x => x.Games.FindAsync(It.IsAny<int>()), Times.Once);
            _gameContextMock.Verify(x => x.Games.Remove(It.IsAny<GameModel>()), Times.Never);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            Assert.IsNull(deleteResult);
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
