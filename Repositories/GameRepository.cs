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

        public async Task<ActionResult<IEnumerable<GameModel>>> GetGames()
        {
            return await _gameContext.Games.ToListAsync();
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
            if (newGame.PublisherId == null && newGame.PublisherName != null)
            {
                var publisher = _publisherContext.Publishers.Where(x => x.PublisherName == newGame.PublisherName).FirstOrDefault();
                if (publisher != null)
                {
                    newGame.PublisherId = publisher.PublisherId;
                }
                else
                {
                    _publisherContext.Publishers.Add(new PublisherModel { PublisherName = newGame.PublisherName });
                    var addedPublisher = _publisherContext.Publishers.Where(x => x.PublisherName == newGame.PublisherName).FirstOrDefault();
                    if (addedPublisher != null) // Should never be null
                    {
                        newGame.PublisherId = addedPublisher.PublisherId;
                    }
                }
            }

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

        public bool GameExists(int id)
        {
            return _gameContext.Games.Any(e => e.GameId == id);
        }
    }
}
