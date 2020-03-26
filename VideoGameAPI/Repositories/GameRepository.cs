using VideoGameAPI.Models;
using VideoGameAPI.Contexts;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace VideoGameAPI.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameContext _gameContext;
        private readonly PublisherContext _publisherContext;
        private readonly ConsoleContext _consoleContext;

        public GameRepository(GameContext gameContext, PublisherContext publisherContext, ConsoleContext consoleContext)
        {
            _gameContext = gameContext;
            _publisherContext = publisherContext;
            _consoleContext = consoleContext;
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
                        ConsoleName = console == null ? null : console.ConsoleName
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

            return gameModel;
        }

        public async Task UpdateGame(GameModel gameModel)
        {
            _gameContext.SetModified(gameModel);

            await _gameContext.SaveChangesAsync();
        }

        public async Task<GameModel> Add(GameModel newGame)
        {
            if(newGame.PublisherId == null && newGame.PublisherName != null) newGame.PublisherId = AddOrRetrievePublisher(newGame.PublisherName);
            // Similar functionality for Console purposefully omitted

            _gameContext.Games.Add(newGame);
            await _gameContext.SaveChangesAsync();

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

        public bool GameExists(int id)
        {
            return _gameContext.Games.Any(e => e.GameId == id);
        }
    }
}
