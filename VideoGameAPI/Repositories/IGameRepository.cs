using VideoGameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VideoGameAPI.Repositories
{
    public interface IGameRepository
    {
        public IEnumerable<GameModel> GetGames();
        public Task<ActionResult<GameModel>> GetGameById(int id);
        public Task UpdateGame(GameModel gameModel);
        public Task<GameModel> Add(GameModel newGame);
        public Task<GameModel> DeleteGame(int id);
        public bool GameExists(int id);
        public Task ImportCSV();
    }
}
