using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geocaches.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeocachingApi.Controllers {
    //[Route ("api/[items]")]
    [ApiController]
    public class ItemsController : ControllerBase {
        private readonly AppDbContext _context;

        public ItemsController (AppDbContext context) {
            _context = context;
        }

        // GET: api/items
        [HttpGet ("/api/items")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItem () {
            return await _context.Item.ToListAsync ();
        }

        // GET: api/items/5
        [HttpGet ("/api/items/{id}")]
        public async Task<ActionResult<Item>> GetItem (int id) {
            var item = await _context.Item.FindAsync (id);

            if (item == null) {
                return NotFound ();
            }

            return item;
        }

        // PUT: api/items/id
        [HttpPut ("api/items/{id}")]
        public async Task<IActionResult> PutItem (int id, Item item) {
            if (id != item.Id) {
                return BadRequest ();
            }

            _context.Entry (item).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync ();
            } catch (DbUpdateConcurrencyException) {
                if (!ItemExists (id)) {
                    return NotFound ();
                } else {
                    throw;
                }
            }

            return NoContent ();
        }

        // POST: api/geocaches/:id
        [HttpPost ("/api/items")]
        public async Task<ActionResult<Item>> PostItem (Item item) {
            try {
                if (!ModelState.IsValid) {
                    return BadRequest ("Model state is not valid.");
                }

                DateTime today = DateTime.Today;
                var timeItemAdded = today.ToString ("g");

                var GeocacheId = new Geocache ();

                var requestBody = new Item {
                    Name = item.Name,
                    Geocache = GeocacheId,
                    isActive = Convert.ToDateTime (timeItemAdded)
                };

                _context.Item.Add (requestBody);
                await _context.SaveChangesAsync ();

                return Ok ($"Item created: { requestBody.Name }");
            } catch (Exception) {
                return StatusCode (StatusCodes.Status500InternalServerError, "Error creating new Item record ");
            }

        }

        // DELETE: api/items/id
        [HttpDelete ("/api/items/{id}")]
        public async Task<IActionResult> DeleteItem (int id) {
            var item = await _context.Item.FindAsync (id);
            if (item == null) {
                return NotFound ();
            }

            _context.Item.Remove (item);
            await _context.SaveChangesAsync ();

            return NoContent ();
        }

        private bool ItemExists (int id) => _context.Item.Any (e => e.Id == id);

    }
}