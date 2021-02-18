using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameAPI.Models;
using VideoGameAPI.Contexts;

namespace VideoGameAPI.Controllers
{
    [Route("api/Consoles")]
    [ApiController]
    public class ConsoleController : ControllerBase
    {
        private readonly VideoGameContext _context;

        public ConsoleController(VideoGameContext context)
        {
            _context = context;
        }

        // GET: api/Console
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsoleModel>>> GetConsoles()
        {
            return await _context.Consoles.ToListAsync();
        }

        // GET: api/Console/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConsoleModel>> GetConsoleModel(int id)
        {
            var consoleModel = await _context.Consoles.FindAsync(id);

            if (consoleModel == null)
            {
                return NotFound();
            }

            return consoleModel;
        }

        // PUT: api/Console/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsoleModel(int id, ConsoleModel consoleModel)
        {
            if (id != consoleModel.ConsoleId)
            {
                return BadRequest();
            }

            _context.Entry(consoleModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsoleModelExists(id))
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

        // POST: api/Console
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ConsoleModel>> PostConsoleModel(ConsoleModel consoleModel)
        {
            _context.Consoles.Add(consoleModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConsoleModel", new { id = consoleModel.ConsoleId }, consoleModel);
        }

        // DELETE: api/Console/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ConsoleModel>> DeleteConsoleModel(int id)
        {
            var consoleModel = await _context.Consoles.FindAsync(id);
            if (consoleModel == null)
            {
                return NotFound();
            }

            _context.Consoles.Remove(consoleModel);
            await _context.SaveChangesAsync();

            return consoleModel;
        }

        private bool ConsoleModelExists(int id)
        {
            return _context.Consoles.Any(e => e.ConsoleId == id);
        }
    }
}
