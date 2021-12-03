using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geocaches.Models;
using Microsoft.AspNetCore.Http;
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

        private bool GeocacheExists (int id) => _context.Geocache.Any (e => e.Id == id);
    }
}