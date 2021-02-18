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
    [Route("api/Genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly VideoGameContext _context;

        public GenreController(VideoGameContext context)
        {
            _context = context;
        }

        // GET: api/Genre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreModel>>> GetGenres()
        {
            return await _context.Genres.ToListAsync();
        }

        // GET: api/Genre/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreModel>> GetGenreModel(int id)
        {
            var genreModel = await _context.Genres.FindAsync(id);

            if (genreModel == null)
            {
                return NotFound();
            }

            return genreModel;
        }

        // PUT: api/Genre/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenreModel(int id, GenreModel genreModel)
        {
            if (id != genreModel.GenreId)
            {
                return BadRequest();
            }

            _context.Entry(genreModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreModelExists(id))
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

        // POST: api/Genre
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<GenreModel>> PostGenreModel(GenreModel genreModel)
        {
            _context.Genres.Add(genreModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenreModel", new { id = genreModel.GenreId }, genreModel);
        }

        // DELETE: api/Genre/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GenreModel>> DeleteGenreModel(int id)
        {
            var genreModel = await _context.Genres.FindAsync(id);
            if (genreModel == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genreModel);
            await _context.SaveChangesAsync();

            return genreModel;
        }

        private bool GenreModelExists(int id)
        {
            return _context.Genres.Any(e => e.GenreId == id);
        }
    }
}
