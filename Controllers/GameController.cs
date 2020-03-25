using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameAPI.Repositories;
using VideoGameAPI.Models;

namespace VideoGameAPI.Controllers
{
    [Route("api/Games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _repository;

        public GameController(IGameRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Game
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameModel>>> GetGames()
        {
            return new ActionResult<IEnumerable<GameModel>>(await _repository.GetGames());
        }

        // GET: api/Game/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameModel>> GetGame(int id)
        {
            var gameModel = await _repository.GetGameById(id);

            if (gameModel == null)
            {
                return NotFound();
            }

            return gameModel;
        }

        // PUT: api/Game/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameModel gameModel)
        {
            if (id != gameModel.GameId)
            {
                return BadRequest();
            }

            try
            {
                await _repository.UpdateGame(gameModel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repository.GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Game
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<GameModel>> PostGame(GameModel gameModel)
        {
            var gameAdded = await _repository.Add(gameModel); // Will potentially make alterations

            return CreatedAtAction("GetGame", new { id = gameAdded.GameId }, gameAdded);
        }

        // DELETE: api/Game/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GameModel>> DeleteGame(int id)
        {
            var gameModel = await _repository.DeleteGame(id);
            if (gameModel == null)
            {
                return NotFound();
            }

            return gameModel;
        }
    }
}
