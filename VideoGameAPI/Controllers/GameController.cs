﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameAPI.Repositories;
using VideoGameAPI.Models;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VideoGameAPI.Controllers
{
    [Route("api/Games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _repository;
        private readonly IConfiguration _config;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameRepository repository, IConfiguration config, ILogger<GameController> logger)
        {
            _repository = repository;
            _config = config;
            _logger = logger;
        }

        // GET: api/Game
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameModel>>> GetGames()
        {
            return new ActionResult<IEnumerable<GameModel>>(_repository.GetGames());
        }

        // GET: api/Game/Console/Genesis
        [HttpGet("Console/{consoleName}")]
        public async Task<ActionResult<IEnumerable<GameModel>>> GetGamesByConsoleName(string consoleName)
        {
            return new ActionResult<IEnumerable<GameModel>>(_repository.GetGamesByConsoleName(consoleName));
        }

        // GET: api/Game/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameModel>> GetGame(int id)
        {
            _logger.LogInformation($"Call to GameGame() with id {id}");

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

            return Ok(new { message = $"Game ID {id} updated" });
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

        // POST: api/Game/ImportCSV
        // Upload and import CSV file
        [HttpPost("ImportCSV")]
        public async Task<IActionResult> ImportCSV([FromForm] IFormFile file)
        {
            if(file == null)
            {
                return StatusCode(400, "No file provided");
            }

            var filePath = String.Empty;

            if(file.Length > 5000000)
            {
                return StatusCode(413, "File must be 5MB or less");
            }

            if(file.Length > 0 && file.ContentType == "text/csv")
            {
                filePath = Path.Combine(_config.GetSection("StoredFilesPath").Value, Path.GetRandomFileName());

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            else
            {
                return StatusCode(415);
            }

            try
            {
                await _repository.ImportCSV(filePath);
            }
            catch (FileNotFoundException)
            {
                return StatusCode(500, "Error after uploading file");
            }

            return Ok(new { size = file.Length, message = "CSV file imported" });
        }
    }
}
