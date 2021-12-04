using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Geocaches.Models {
    public class Geocache {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coordinate { get; set; }

        public List <Item> Items { get; set; }
    }
}