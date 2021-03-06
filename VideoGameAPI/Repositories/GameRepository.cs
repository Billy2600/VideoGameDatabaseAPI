﻿using VideoGameAPI.Models;
using VideoGameAPI.Contexts;
using VideoGameAPI.Helpers;
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
        private readonly VideoGameContext _videoGameContext;
        private readonly IFileManager _fileManager;

        public GameRepository(VideoGameContext videoGameContext,
            IFileManager fileManager)
        {
            _videoGameContext = videoGameContext;
            _fileManager = fileManager;
        }

        public IEnumerable<GameModel> GetGames()
        {
            // Convert these to lists so that we can do a join on them without error
            var games = _videoGameContext.Games.ToList();
            var publishers = _videoGameContext.Publishers.ToList();
            var consoles = _videoGameContext.Consoles.ToList();

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

        public IEnumerable<GameModel> GetGamesByConsoleName(string consoleName)
        {
            var games = _videoGameContext.Games.ToList();
            var publishers = _videoGameContext.Publishers.ToList();
            var consoles = _videoGameContext.Consoles.ToList();

            return (from game in games
                    join publisher in publishers on game.PublisherId equals publisher.PublisherId into pub
                    from publisher in pub.DefaultIfEmpty()
                    join console in consoles on game.ConsoleId equals console.ConsoleId into con
                    from console in con.DefaultIfEmpty()
                    where console.ConsoleName == consoleName
                    select game + new GameModel
                    {
                        PublisherName = publisher == null ? null : publisher.PublisherName,
                        ConsoleName = console == null ? null : console.ConsoleName,
                        Genres = GetGenresForGame(game.GameId) // Might be more efficient but more complex ways to do this
                    }).ToList();
        }

        public async Task<ActionResult<GameModel>> GetGameById(int id)
        {
            var gameModel = await _videoGameContext.Games.FindAsync(id);
            if (gameModel.PublisherId != null)
            {
                var publisher = _videoGameContext.Publishers.Where(x => x.PublisherId == gameModel.PublisherId).FirstOrDefault();
                if (publisher != null) gameModel.PublisherName = publisher.PublisherName;
            }
            if (gameModel.ConsoleId != null)
            {
                var Console = _videoGameContext.Consoles.Where(x => x.ConsoleId == gameModel.ConsoleId).FirstOrDefault();
                if (Console != null) gameModel.ConsoleName = Console.ConsoleName;
            }

            gameModel.Genres = GetGenresForGame(gameModel.GameId);

            return gameModel;
        }

        public async Task UpdateGame(GameModel updatedGame)
        {
            if (updatedGame.PublisherId == null && updatedGame.PublisherName != null) updatedGame.PublisherId = AddOrRetrievePublisher(updatedGame.PublisherName);
            if (updatedGame.Genres != null && updatedGame.Genres.Count > 0) MapGenresToGameByNames(updatedGame.GameId, updatedGame.Genres);
            _videoGameContext.SetModified(updatedGame);

            await _videoGameContext.SaveChangesAsync();
        }

        public async Task<GameModel> Add(GameModel newGame)
        {
            if(newGame.PublisherId == null && newGame.PublisherName != null) newGame.PublisherId = AddOrRetrievePublisher(newGame.PublisherName);
            // Similar functionality for Console purposefully omitted

            _videoGameContext.Games.Add(newGame);
            await _videoGameContext.SaveChangesAsync();
            // Need to do this after adding it, so we have its ID
            if (newGame.Genres != null && newGame.Genres.Count > 0) MapGenresToGameByNames(newGame.GameId, newGame.Genres);

            return newGame;
        }

        public async Task<GameModel> DeleteGame(int id)
        {
            var gameModel = await _videoGameContext.Games.FindAsync(id);
            if (gameModel == null)
            {
                return null;
            }

            _videoGameContext.Games.Remove(gameModel);
            await _videoGameContext.SaveChangesAsync();

            return gameModel;
        }

        private int AddOrRetrievePublisher(string publisherName)
        {
            if (_videoGameContext.Publishers.Where(x => x.PublisherName == publisherName).FirstOrDefault() == null)
            {
                var newPublisher = new PublisherModel { PublisherName = publisherName };
                _videoGameContext.Publishers.Add(newPublisher);
                _videoGameContext.SaveChanges();

                return newPublisher.PublisherId;
            }
            else
            {
                return _videoGameContext.Publishers.Where(x => x.PublisherName == publisherName).FirstOrDefault().PublisherId;
            }
        }

        private List<string> GetGenresForGame(int gameId)
        {
            var gameGenres = _videoGameContext.GamesGenres.ToList();
            var genres = _videoGameContext.Genres.ToList();

            return (from gameGenre in gameGenres
                        join genre in genres on gameGenre.GenreId equals genre.GenreId
                        where gameGenre.GameId == gameId
                        select genre.GenreName).ToList();
        }

        private void MapGenresToGameByNames(int gameId, List<string> genreNames)
        {
            foreach (var genre in genreNames)
            {
                var genreEntity = _videoGameContext.Genres.Where(x => x.GenreName == genre).FirstOrDefault();
                if (genreEntity == null)
                {
                    var newGenre = new GenreModel() { GenreName = genre };
                    _videoGameContext.Genres.Add(newGenre);
                    _videoGameContext.SaveChanges();

                    _videoGameContext.GamesGenres.Add(new GameGenreModel() { GameId = gameId, GenreId = newGenre.GenreId });
                    _videoGameContext.SaveChanges();
                }
                else
                {
                    _videoGameContext.GamesGenres.Add(new GameGenreModel() { GameId = gameId, GenreId = genreEntity.GenreId });
                    _videoGameContext.SaveChanges();
                }
            }
        }

        public bool GameExists(int id)
        {
            return _videoGameContext.Games.Any(e => e.GameId == id);
        }

        public async Task ImportCSV(string filePath)
        {
            if(!_fileManager.FileExists(filePath))
            {
                throw new FileNotFoundException();
            }

            using (var reader = _fileManager.StreamReader(filePath))
            using (var csv = _fileManager.CsvReader(reader, CultureInfo.InvariantCulture))
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
                        ConsoleId = _videoGameContext.GetConsoleIdByName(record.Console)
                    };

                    await Add(newGame);
                }
            }
        }
    }
}
