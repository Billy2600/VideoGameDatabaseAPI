using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameAPI.Contexts;
using VideoGameAPI.Models;

namespace VideoGameAPI.Controllers
{
    [Route("api/Games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameContext _context;

        public GameController(GameContext context)
        {
            _context = context;
        }

        // GET: api/Game
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameModel>>> GetGames()
        {
            return await _context.Games.ToListAsync();
        }

        // GET: api/Game/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameModel>> GetGameModel(int id)
        {
            var gameModel = await _context.Games.FindAsync(id);

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
        public async Task<IActionResult> PutGameModel(int id, GameModel gameModel)
        {
            if (id != gameModel.GameId)
            {
                return BadRequest();
            }

            _context.Entry(gameModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameModelExists(id))
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
        public async Task<ActionResult<GameModel>> PostGameModel(GameModel gameModel)
        {
            _context.Games.Add(gameModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameModel", new { id = gameModel.GameId }, gameModel);
        }

        // DELETE: api/Game/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GameModel>> DeleteGameModel(int id)
        {
            var gameModel = await _context.Games.FindAsync(id);
            if (gameModel == null)
            {
                return NotFound();
            }

            _context.Games.Remove(gameModel);
            await _context.SaveChangesAsync();

            return gameModel;
        }

        private bool GameModelExists(int id)
        {
            return _context.Games.Any(e => e.GameId == id);
        }
    }
}
