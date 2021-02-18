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
    [Route("api/Publishers")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly VideoGameContext _context;

        public PublisherController(VideoGameContext context)
        {
            _context = context;
        }

        // GET: api/Publisher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherModel>>> GetPublishers()
        {
            return await _context.Publishers.ToListAsync();
        }

        // GET: api/Publisher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherModel>> GetPublisherModel(int id)
        {
            var publisherModel = await _context.Publishers.FindAsync(id);

            if (publisherModel == null)
            {
                return NotFound();
            }

            return publisherModel;
        }

        // PUT: api/Publisher/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisherModel(int id, PublisherModel publisherModel)
        {
            if (id != publisherModel.PublisherId)
            {
                return BadRequest();
            }

            _context.Entry(publisherModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherModelExists(id))
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

        // POST: api/Publisher
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PublisherModel>> PostPublisherModel(PublisherModel publisherModel)
        {
            _context.Publishers.Add(publisherModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisherModel", new { id = publisherModel.PublisherId }, publisherModel);
        }

        // DELETE: api/Publisher/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PublisherModel>> DeletePublisherModel(int id)
        {
            var publisherModel = await _context.Publishers.FindAsync(id);
            if (publisherModel == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisherModel);
            await _context.SaveChangesAsync();

            return publisherModel;
        }

        [HttpGet("GateIdByName/{name}")]
        public async Task<ActionResult<int>> GetIdByName(string name)
        {
            return await _context.GetPublisherIdByName(name);
        }

        private bool PublisherModelExists(int id)
        {
            return _context.Publishers.Any(e => e.PublisherId == id);
        }
    }
}
