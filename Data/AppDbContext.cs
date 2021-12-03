using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Geocaches.Models {
    public class AppDbContext : DbContext {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base (options) {

        }

        public virtual DbSet<Geocache> Geocache { get; set; }
        public virtual DbSet<Item> Item { get; set; }
    }

}