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

                var today = DateTime.Today;

                var GeocacheId = new Geocache ();

                var requestBody = new Item {
                    Name = item.Name,
                    Geocache = (int) GeocacheId.Id,
                    isActive = today,
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

        // 5. Only active items should be allowed to be moved, and items cannot be moved to a geocache that already contains 3 or more items.
        [HttpPatch ("/api/geocaches/{id}")]
        public async Task<ActionResult<Item>> MoveItem (Item item) {

            var GeocacheItems = new Geocache ();

            //items no longer active after 90 days
            DateTime isActive = DateTime.Now;
            DateTime itemInactivePeriod = isActive.AddMonths (3);
            if (itemInactivePeriod > DateTime.Now) {
                Console.WriteLine ("Item has been on longer than 90 days and is no longer active.Cannot move to other Geocache.");
                return null; //no data is available as we cannot add inactive items
            }
            //items cannot be moved to a geocache that already contains 3 or more items
            if (GeocacheItems.Items.Count () > 3) {
                Console.WriteLine ("Cannot store more than three items in this Geocache ");
                return null;
            }

            _context.Item.Add (item);
            await _context.SaveChangesAsync ();

            return Ok ($"Item added to Geocache: { GeocacheItems.Items }");
        }

        private bool ItemExists (int id) => _context.Item.Any (e => e.Id == id);

    }
}