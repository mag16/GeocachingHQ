using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Geocaches.Models {
    //attribute to set only unique item names.
    [Index (nameof (Name), IsUnique = true)]
    public class Item {
        public int Id { get; set; }

        [Required]
        [MaxLength (50, ErrorMessage = "Name field cannot exceed 50 characters")] //set limit on name field to 50 chars max.
        [RegularExpression ("^[A-Z0-9]*$", ErrorMessage = "Name field only allows letters, numbers and spaces.")]
        public string Name { get; set; }

        public int Geocache { get; set; }

        public DateTime isActive { get; set; }
    }
}