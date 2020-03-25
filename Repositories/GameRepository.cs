using VideoGameAPI.Models;
using VideoGameAPI.Contexts;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VideoGameAPI.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameContext _gameContext;
        private readonly PublisherContext _publisherContext;

        public GameRepository(GameContext gameContext, PublisherContext publisherContext)
        {
            _gameContext = gameContext;
            _publisherContext = publisherContext;
        }

        public async Task<IEnumerable<GameModel>> GetGames()
        {
            // Convert these to lists so that we can do a join on them without error
            var games = await _gameContext.Games.ToListAsync();
            var publishers = await _publisherContext.Publishers.ToListAsync();

            return (from game in games
                    join publisher in publishers on game.PublisherId equals publisher.PublisherId
                    select game + new GameModel
                    {
                        PublisherName = publisher.PublisherName
                    }).ToList();
        }

        public async Task<ActionResult<GameModel>> GetGameById(int id)
        {
            var gameModel = await _gameContext.Games.FindAsync(id);

            return gameModel;
        }

        public async Task UpdateGame(GameModel gameModel)
        {
            _gameContext.Entry(gameModel).State = EntityState.Modified;

            await _gameContext.SaveChangesAsync();
        }

        public async Task<GameModel> Add(GameModel newGame)
        {
            if(newGame.PublisherId == null && newGame.PublisherName != null) newGame.PublisherId = AddOrRetrievePublisher(newGame.PublisherName);

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
