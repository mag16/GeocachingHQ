using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geocaches.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeocachingApi.Controllers {
    //[Route ("api/[geocaches]")]
    [ApiController]
    public class GeocachesController : ControllerBase {
        private readonly AppDbContext _context;

        public GeocachesController (AppDbContext context) {
            _context = context;
        }

        // GET: api/Geocaches
        [HttpGet ("api/geocaches")]
        public async Task<ActionResult<IEnumerable<Geocache>>> GetGeocaches () {
            return await _context.Geocache.ToListAsync ();
        }

        // GET: api/geocaches/id
        [HttpGet ("api/geocaches/{id}")]
        public async Task<ActionResult<Geocache>> GetGeocache (int id) {
            var geocache = await _context.Geocache.FindAsync (id);

            if (geocache == null) {
                return NotFound ();
            }

            return geocache;
        }

        // PUT: api/Geocaches/id
        [HttpPut ("api/geocaches/{id}")]
        public async Task<IActionResult> PutGeocache (int id, Geocache geocache) {
            if (id != geocache.Id) {
                return BadRequest ();
            }

            _context.Entry (geocache).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync ();
            } catch (DbUpdateConcurrencyException) {
                if (!GeocacheExists (id)) {
                    return NotFound ();
                } else {
                    throw;
                }
            }

            return NoContent ();
        }

        // POST: api/Geocaches
        [HttpPost ("/api/geocaches")]
        public async Task<ActionResult<Geocache>> PostGeocache (Geocache geocache) {
            try {
                if (!ModelState.IsValid) {
                    return BadRequest ("Model state is not valid.");
                }

                var requestBody = new Geocache {
                    Name = geocache.Name,
                    Coordinate = geocache.Coordinate,
                    Items = geocache.Items
                };

                _context.Geocache.Add (requestBody);
                await _context.SaveChangesAsync ();

                return Ok ($"Geocache created: {requestBody.Name}");
            } catch (Exception) {
                return StatusCode (StatusCodes.Status500InternalServerError, "Error creating new Geocache record");
            }

            //return CreatedAtAction("GetGeocache", new { id = geocache.Id }, geocache);
        }

        // DELETE: api/Geocahes/5
        [HttpDelete ("api/geocaches/{id}")]
        public async Task<IActionResult> DeleteGeocache (int id) {
            var geocache = await _context.Geocache.FindAsync (id);
            if (geocache == null) {
                return NotFound ();
            }

            _context.Geocache.Remove (geocache);
            await _context.SaveChangesAsync ();

            return NoContent ();
        }

        // 5. Only active items should be allowed to be moved, and items cannot be moved to a geocache that already contains 3 or more items.
        [HttpPatch ("api/geocaches/{id}")]
        public async Task<ActionResult<Geocache>> MoveItems ([FromBody] JsonPatchDocument<Geocache> patchDoc) {

            // //items no longer active after 90 days
            // DateTime isActive = DateTime.Now;
            // DateTime itemInactivePeriod = isActive.AddMonths (3);
            // if (itemInactivePeriod > item.isActive) {
            //     Console.WriteLine ("Item has been on longer than 90 days and is no longer active.Cannot move to other Geocache.");
            //     return null; //no data is available as we cannot add inactive items
            // }
            // //items cannot be moved to a geocache that already contains 3 or more items
            // if (GeocacheItems.Items.Count () > 3) {
            //     Console.WriteLine ("Cannot store more than three items in this Geocache ");
            //     return null;
            // }

            if (patchDoc != null) {
                var geocache = new Geocache ();

                patchDoc.ApplyTo (geocache, ModelState);

                if (!ModelState.IsValid) {
                    return BadRequest (ModelState);
                }

                await _context.SaveChangesAsync ();

                return new ObjectResult (geocache);
            } else {
                return BadRequest (ModelState);
            }
        }

        private bool GeocacheExists (int id) => _context.Geocache.Any (e => e.Id == id);
    }
}