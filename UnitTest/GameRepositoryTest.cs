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
        private GameRepository _gameRepo;

        private Mock<GameContext> _gameContextMock;
        private Mock<PublisherContext> _publisherContextMock;
        private Mock<ConsoleContext> _consoleContextMock;
        private Mock<GameGenreContext> _gameGenreContextMock;
        private Mock<GenreContext> _genreContextMock;

        [TestInitialize]
        public void Initialize()
        {
            _fixture = new Fixture();
            _gameContextMock = new Mock<GameContext>();
            _publisherContextMock = new Mock<PublisherContext>();
            _consoleContextMock = new Mock<ConsoleContext>();
            _gameGenreContextMock = new Mock<GameGenreContext>();
            _genreContextMock = new Mock<GenreContext>();

            _gameRepo = new GameRepository(_gameContextMock.Object, _publisherContextMock.Object, _consoleContextMock.Object, _gameGenreContextMock.Object, _genreContextMock.Object);
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
            var genreDbSet = CreateDbSetMock(new List<GenreModel> { }); // Purposefully empty
            var gameGenreDbSet = CreateDbSetMock(new List<GameGenreModel> { }); // Same

            _gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).ReturnsAsync(testGame);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _consoleContextMock.Setup(x => x.Consoles).Returns(consoleDbSet.Object);
            _genreContextMock.Setup(x => x.Genres).Returns(genreDbSet.Object);
            _gameGenreContextMock.Setup(x => x.GamesGenres).Returns(gameGenreDbSet.Object);

            // Act
            var gameResult = await _gameRepo.GetGameById(testGame.GameId);

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
            var genreDbSet = CreateDbSetMock(new List<GenreModel> { }); // Same
            var gameGenreDbSet = CreateDbSetMock(new List<GameGenreModel> { }); // Same

            _gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).ReturnsAsync(testGame);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _genreContextMock.Setup(x => x.Genres).Returns(genreDbSet.Object);
            _gameGenreContextMock.Setup(x => x.GamesGenres).Returns(gameGenreDbSet.Object);

            // Act
            var gameResult = await _gameRepo.GetGameById(testGame.GameId);

            // Assert
            Assert.AreEqual(testGame.GameId, gameResult.Value.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.Value.GameName);
            Assert.AreEqual(testGame.PublisherId, gameResult.Value.PublisherId);
            Assert.IsNull(testGame.PublisherName);
        }

        [TestMethod]
        public async Task GetGameById_WithGenre()
        {
            // Arrange
            var testGenre = new GenreModel()
            {
                GenreId = _fixture.Create<int>(),
                GenreName = _fixture.Create<string>()
            };

            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>()
            };

            var testGameGenre = new GameGenreModel()
            {
                GameGenreId = _fixture.Create<int>(),
                GameId = testGame.GameId,
                GenreId = testGenre.GenreId
            };

            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { }); // Empty as we're not testing this
            var genreDbSet = CreateDbSetMock(new List<GenreModel> { testGenre }); // Same
            var gameGenreDbSet = CreateDbSetMock(new List<GameGenreModel> { testGameGenre }); // Same

            _gameContextMock.Setup(x => x.Games.FindAsync(It.IsAny<int>())).ReturnsAsync(testGame);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _genreContextMock.Setup(x => x.Genres).Returns(genreDbSet.Object);
            _gameGenreContextMock.Setup(x => x.GamesGenres).Returns(gameGenreDbSet.Object);

            // Act
            var gameResult = await _gameRepo.GetGameById(testGame.GameId);

            // Assert
            Assert.AreEqual(testGame.GameId, gameResult.Value.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.Value.GameName);
            Assert.AreEqual(testGame.Genres.Count, 1);
            Assert.AreEqual(testGame.Genres.First(), testGenre.GenreName);
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
            var genreDbSet = CreateDbSetMock(new List<GenreModel> { }); // Empty as we're not testing this
            var gameGenreDbSet = CreateDbSetMock(new List<GameGenreModel> { }); // Same

            _gameContextMock.Setup(x => x.Games).Returns(gameDbSet.Object);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _consoleContextMock.Setup(x => x.Consoles).Returns(consoleDbSet.Object);
            _genreContextMock.Setup(x => x.Genres).Returns(genreDbSet.Object);
            _gameGenreContextMock.Setup(x => x.GamesGenres).Returns(gameGenreDbSet.Object);

            // Act
            var gameResults = _gameRepo.GetGames();

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
        public void GetGames_WithGenres()
        {
            // Tests only one, because repeated calls to mock DbSets' GetEnumerator() only works on the first try
            // TODO: Figure why that is, and/or out how to work around it

            // Arrange
            var testGenres = new List<GenreModel>()
            {
                new GenreModel()
                {
                    GenreId = _fixture.Create<int>(),
                    GenreName = _fixture.Create<string>()
                },
                new GenreModel()
                {
                    GenreId = _fixture.Create<int>(),
                    GenreName = _fixture.Create<string>()
                }
            };
            // Separate out the names for verifying
            var genreNames = (from genre in testGenres
                             select genre.GenreName).ToList();

            var testGames = new List<GameModel>()
            {
                new GameModel()
                {
                    GameId = _fixture.Create<int>(),
                    GameName = _fixture.Create<string>()
                }
            };

            var testGameGenres = new List<GameGenreModel>();

            testGameGenres.Add(new GameGenreModel()
            {
                GameGenreId = _fixture.Create<int>(),
                GameId = testGames.First().GameId,
                GenreId = testGenres.First().GenreId
            });

            testGameGenres.Add(new GameGenreModel()
            {
                GameGenreId = _fixture.Create<int>(),
                GameId = testGames.First().GameId,
                GenreId = testGenres.Last().GenreId
            });
            

            var gameDbSet = CreateDbSetMock(testGames);
            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { }); // Empty because we're not testing this
            var consoleDbSet = CreateDbSetMock(new List<ConsoleModel>() { }); // Same
            var genreDbSet = CreateDbSetMock(testGenres);
            var gameGenreDbSet = CreateDbSetMock(testGameGenres);

            _gameContextMock.Setup(x => x.Games).Returns(gameDbSet.Object);
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _consoleContextMock.Setup(x => x.Consoles).Returns(consoleDbSet.Object);
            _genreContextMock.Setup(x => x.Genres).Returns(genreDbSet.Object);
            _gameGenreContextMock.Setup(x => x.GamesGenres).Returns(gameGenreDbSet.Object);

            // Act
            var gameResults = _gameRepo.GetGames();

            // Assert
            Assert.AreEqual(testGames.Count(), gameResults.Count());
            for (int i = 0; i < testGames.Count(); i++)
            {
                var testGame = testGames.ElementAt(i);
                var gameResult = gameResults.ElementAt(i);

                Assert.AreEqual(testGame.GameId, gameResult.GameId);
                Assert.AreEqual(testGame.GameName, gameResult.GameName);
                CollectionAssert.AreEqual(genreNames, gameResult.Genres);
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

            _gameContextMock.Setup(x => x.SetModified(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

            // Act
            await _gameRepo.UpdateGame(testGame);

            // Assert
            _gameContextMock.Verify(x => x.SetModified(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateGame_AutoAddPublisher()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                PublisherName = _fixture.Create<string>()
            };

            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() { });

            _gameContextMock.Setup(x => x.SetModified(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);

            // Act
            await _gameRepo.UpdateGame(testGame);

            // Assert
            _gameContextMock.Verify(x => x.SetModified(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _publisherContextMock.Verify(x => x.Publishers.Add(It.IsAny<PublisherModel>()), Times.Once);
            _publisherContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public async Task UpdateGame_AutoAddGenres()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                Genres = new List<string>()
                {
                    _fixture.Create<string>()
                }
            };

            var genreId = _fixture.Create<int>();
            var addedGenreId = -1;

            var genreDbSet = CreateDbSetMock(new List<GenreModel>() { });
            var gameGenreDbSet = CreateDbSetMock(new List<GameGenreModel>() { });

            _gameContextMock.Setup(x => x.Games.Add(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            _genreContextMock.Setup(x => x.Genres).Returns(genreDbSet.Object);
            _genreContextMock.Setup(x => x.Genres.Add(It.IsAny<GenreModel>())).Callback<GenreModel>(x => x.GenreId = genreId);
            _gameGenreContextMock.Setup(x => x.GamesGenres).Returns(gameGenreDbSet.Object);
            _gameGenreContextMock.Setup(x => x.GamesGenres.Add(It.IsAny<GameGenreModel>())).Callback<GameGenreModel>(x => addedGenreId = x.GenreId);

            // Act
            await _gameRepo.UpdateGame(testGame);

            // Assert
            _gameContextMock.Verify(x => x.SetModified(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _genreContextMock.Verify(x => x.SaveChanges(), Times.Once);
            _genreContextMock.Verify(x => x.Genres.Add(It.IsAny<GenreModel>()), Times.Once);
            _gameGenreContextMock.Verify(x => x.SaveChanges(), Times.Once);
            _gameGenreContextMock.Verify(x => x.GamesGenres.Add(It.IsAny<GameGenreModel>()), Times.Once);

            Assert.AreEqual(genreId, addedGenreId);
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

            // Act
            var gameResult = await _gameRepo.Add(testGame);

            // Assert
            _gameContextMock.Verify(x => x.Games.Add(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(testGame.GameId, gameResult.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.GameName);
            Assert.AreEqual(testPublisher.PublisherId, gameResult.PublisherId); // Should've automatically mapped Id based on name
            Assert.AreEqual(testPublisher.PublisherName, gameResult.PublisherName);
        }

        [TestMethod]
        public async Task Add_AutoAddPublisher()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                PublisherName = _fixture.Create<string>()
            };

            var publisherId = _fixture.Create<int>();

            var publisherDbSet = CreateDbSetMock(new List<PublisherModel>() {  });

            _gameContextMock.Setup(x => x.Games.Add(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            _publisherContextMock.Setup(x => x.Publishers).Returns(publisherDbSet.Object);
            _publisherContextMock.Setup(x => x.Publishers.Add(It.IsAny<PublisherModel>())).Callback<PublisherModel>(x => x.PublisherId = publisherId);

            // Act
            var gameResult = await _gameRepo.Add(testGame);

            // Assert
            _gameContextMock.Verify(x => x.Games.Add(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(testGame.GameId, gameResult.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.GameName);
            Assert.AreEqual(testGame.PublisherName, gameResult.PublisherName);
            Assert.AreEqual(publisherId, gameResult.PublisherId); // Should've been mapped to publisher that was added
            _publisherContextMock.Verify(x => x.Publishers.Add(It.IsAny<PublisherModel>()), Times.Once);
            _publisherContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public async Task Add_AutoAddGenres()
        {
            // Arrange
            var testGame = new GameModel()
            {
                GameId = _fixture.Create<int>(),
                GameName = _fixture.Create<string>(),
                Genres = new List<string>()
                {
                    _fixture.Create<string>()
                }
            };

            var genreId = _fixture.Create<int>();
            var addedGenreId = -1;

            var genreDbSet = CreateDbSetMock(new List<GenreModel>() { });
            var gameGenreDbSet = CreateDbSetMock(new List<GameGenreModel>() { });

            _gameContextMock.Setup(x => x.Games.Add(It.IsAny<GameModel>()));
            _gameContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            _genreContextMock.Setup(x => x.Genres).Returns(genreDbSet.Object);
            _genreContextMock.Setup(x => x.Genres.Add(It.IsAny<GenreModel>())).Callback<GenreModel>(x => x.GenreId = genreId);
            _gameGenreContextMock.Setup(x => x.GamesGenres).Returns(gameGenreDbSet.Object);
            _gameGenreContextMock.Setup(x => x.GamesGenres.Add(It.IsAny<GameGenreModel>())).Callback<GameGenreModel>(x => addedGenreId = x.GenreId);

            // Act
            var gameResult = await _gameRepo.Add(testGame);

            // Assert
            _gameContextMock.Verify(x => x.Games.Add(It.IsAny<GameModel>()), Times.Once);
            _gameContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _genreContextMock.Verify(x => x.SaveChanges(), Times.Once);
            _genreContextMock.Verify(x => x.Genres.Add(It.IsAny<GenreModel>()), Times.Once);
            _gameGenreContextMock.Verify(x => x.SaveChanges(), Times.Once);
            _gameGenreContextMock.Verify(x => x.GamesGenres.Add(It.IsAny<GameGenreModel>()), Times.Once);

            Assert.AreEqual(testGame.GameId, gameResult.GameId);
            Assert.AreEqual(testGame.GameName, gameResult.GameName);
            Assert.AreEqual(testGame.Genres.First(), gameResult.Genres.First());
            Assert.AreEqual(genreId, addedGenreId);
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

            // Act
            var deleteResult = await _gameRepo.DeleteGame(testGame.GameId);

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

            // Act
            var deleteResult = await _gameRepo.DeleteGame(invalidGameId);

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
