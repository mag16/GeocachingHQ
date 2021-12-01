using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Geocaches.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeocachingApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase {
        private readonly GeocachesContext _context;

        public ItemsController (GeocachesContext context) {
            _context = context;
        }

        // GET: api/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItem () {
            return await _context.Item.ToListAsync ();
        }

        // GET: api/Geocahes/5
        [HttpGet ("/api/items/{id}")]
        public async Task<ActionResult<Item>> GetItem (int id) {
            var item = await _context.Item.FindAsync (id);

            if (item == null) {
                return NotFound ();
            }

            return item;
        }

        // PUT: api/items/id
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut ("{id}")]
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost ("/api/items/")]
        public async Task<ActionResult<Item>> PostItem (Item item) {

            try {
                if (!ModelState.IsValid) {
                    return BadRequest ("Model state is not valid.");
                }

                var today = DateTime.Today;

                var GeocacheId = new Geocache();

                var requestBody = new Item {
                    Name = item.Name,
                    Geocache = (int) GeocacheId.Id,
                    isActive = today,
                };

                //Regular expression to check for letters,numbers,spaces in Name field.
                Regex reg = new Regex ("^[A-Z0-9]*$");
                Match match = reg.Match (item.Name);

                //TODO: item name should be unique(set in items model), contain 50 chars max and only allow letters,numbers,spaces
                int nameLength = item.Name.ToCharArray ().Count ();
                if (nameLength > 50) {
                    Console.WriteLine ("Item name cannot be longer than 50 characters long.");
                }

                if (match.Success) {
                    Console.WriteLine ("Item name: " + match.Value);
                } else {
                    return null;
                }

                _context.Item.Add (requestBody);
                await _context.SaveChangesAsync();

                return Ok ($"Item created: { requestBody.Name }");
            } catch (Exception) {
                return StatusCode (StatusCodes.Status500InternalServerError, "Error creating new Item record ");
            }

        }

        // DELETE: api/items/id
        [HttpDelete ("{id}")]
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