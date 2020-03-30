using VideoGameAPI.Models;
using VideoGameAPI.Contexts;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;
using System;

namespace VideoGameAPI.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameContext _gameContext;
        private readonly PublisherContext _publisherContext;
        private readonly ConsoleContext _consoleContext;
        private readonly GameGenreContext _gameGenreContext;
        private readonly GenreContext _genreContext;

        public GameRepository(GameContext gameContext, PublisherContext publisherContext, ConsoleContext consoleContext, GameGenreContext gameGenreContext, GenreContext genreContext)
        {
            _gameContext = gameContext;
            _publisherContext = publisherContext;
            _consoleContext = consoleContext;
            _gameGenreContext = gameGenreContext;
            _genreContext = genreContext;
        }

        public IEnumerable<GameModel> GetGames()
        {
            // Convert these to lists so that we can do a join on them without error
            var games = _gameContext.Games.ToList();
            var publishers = _publisherContext.Publishers.ToList();
            var consoles = _consoleContext.Consoles.ToList();

            return (from game in games
                    // Doing the equivalent of a SQL left outer join
                    // We want to include rows where PublisherId and ConsoleId is null
                    join publisher in publishers on game.PublisherId equals publisher.PublisherId into pub
                    from publisher in pub.DefaultIfEmpty()
                    join console in consoles on game.ConsoleId equals console.ConsoleId into con
                    from console in con.DefaultIfEmpty()
                    select game + new GameModel
                    {
                        PublisherName = publisher == null ? null : publisher.PublisherName,
                        ConsoleName = console == null ? null : console.ConsoleName,
                        Genres = GetGenresForGame(game.GameId) // Might be more efficient but more complex ways to do this
                    }).ToList();
        }

        public async Task<ActionResult<GameModel>> GetGameById(int id)
        {
            var gameModel = await _gameContext.Games.FindAsync(id);
            if (gameModel.PublisherId != null)
            {
                var publisher = _publisherContext.Publishers.Where(x => x.PublisherId == gameModel.PublisherId).FirstOrDefault();
                if (publisher != null) gameModel.PublisherName = publisher.PublisherName;
            }
            if (gameModel.ConsoleId != null)
            {
                var Console = _consoleContext.Consoles.Where(x => x.ConsoleId == gameModel.ConsoleId).FirstOrDefault();
                if (Console != null) gameModel.ConsoleName = Console.ConsoleName;
            }

            gameModel.Genres = GetGenresForGame(gameModel.GameId);

            return gameModel;
        }

        public async Task UpdateGame(GameModel updatedGame)
        {
            if (updatedGame.PublisherId == null && updatedGame.PublisherName != null) updatedGame.PublisherId = AddOrRetrievePublisher(updatedGame.PublisherName);
            if (updatedGame.Genres != null && updatedGame.Genres.Count > 0) AddNewGenres(updatedGame.GameId, updatedGame.Genres);
            _gameContext.SetModified(updatedGame);

            await _gameContext.SaveChangesAsync();
        }

        public async Task<GameModel> Add(GameModel newGame)
        {
            if(newGame.PublisherId == null && newGame.PublisherName != null) newGame.PublisherId = AddOrRetrievePublisher(newGame.PublisherName);
            // Similar functionality for Console purposefully omitted

            _gameContext.Games.Add(newGame);
            await _gameContext.SaveChangesAsync();
            // Need to do this after adding it, so we have its ID
            if (newGame.Genres != null && newGame.Genres.Count > 0) AddNewGenres(newGame.GameId, newGame.Genres);

            return newGame;
        }

        public async Task<GameModel> DeleteGame(int id)
        {
            var gameModel = await _gameContext.Games.FindAsync(id);
            if (gameModel == null)
            {
                return null;
            }

            _gameContext.Games.Remove(gameModel);
            await _gameContext.SaveChangesAsync();

            return gameModel;
        }

        private int AddOrRetrievePublisher(string publisherName)
        {
            if (_publisherContext.Publishers.Where(x => x.PublisherName == publisherName).FirstOrDefault() == null)
            {
                var newPublisher = new PublisherModel { PublisherName = publisherName };
                _publisherContext.Publishers.Add(newPublisher);
                _publisherContext.SaveChanges();

                return newPublisher.PublisherId;
            }
            else
            {
                return _publisherContext.Publishers.Where(x => x.PublisherName == publisherName).FirstOrDefault().PublisherId;
            }
        }

        private List<string> GetGenresForGame(int gameId)
        {
            var gameGenres = _gameGenreContext.GamesGenres.ToList();
            var genres = _genreContext.Genres.ToList();

            return (from gameGenre in gameGenres
                        join genre in genres on gameGenre.GenreId equals genre.GenreId
                        where gameGenre.GameId == gameId
                        select genre.GenreName).ToList();
        }

        private void AddNewGenres(int gameId, List<string> genreNames)
        {
            foreach (var genre in genreNames)
            {
                if (_genreContext.Genres.Where(x => x.GenreName == genre).FirstOrDefault() == null)
                {
                    var newGenre = new GenreModel() { GenreName = genre };
                    _genreContext.Genres.Add(newGenre);
                    _genreContext.SaveChanges();

                    _gameGenreContext.GamesGenres.Add(new GameGenreModel() { GameId = gameId, GenreId = newGenre.GenreId });
                    _gameGenreContext.SaveChanges();
                }
            }
        }

        public bool GameExists(int id)
        {
            return _gameContext.Games.Any(e => e.GameId == id);
        }

        public async Task ImportCSV()
        {
            using (var reader = new StreamReader("C:\\projects\\VideoGameAPI\\csv\\Genesis.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<GameCSV>();

                foreach (var record in records)
                {
                    var newGame = new GameModel()
                    {
                        GameName = record.Title,
                        ReleaseDate = DateTime.Parse(record.Year.ToString() + "-01-01"), // We only have a year, but you can't have a DateTime without month and day
                        PublisherName = record.Publisher.Split(',').First(), // Only using the first one, for now
                        Genres = record.Genre.Split(',').Select(x => x.Trim()).ToList(),
                        ConsoleId = _consoleContext.Consoles.Where(x => x.ConsoleName == record.Console).FirstOrDefault().ConsoleId
                    };

                    await Add(newGame);
                }
            }
        }
    }
}
