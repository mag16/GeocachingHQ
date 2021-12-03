using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Geocaches.Models {
    public class GeocachesContext : DbContext {
        public GeocachesContext (DbContextOptions<GeocachesContext> options) : base (options) {

        }

        public virtual DbSet<Geocache> Geocache { get; set; }
       // public virtual DbSet<Item> Item { get; set; }
    }

}