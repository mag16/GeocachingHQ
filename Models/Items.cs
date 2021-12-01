using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Geocaches.Models 
{
    //attribute to set only unique item names.
    [Index (nameof (Name), IsUnique = true)]
    public class Item 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Geocache { get; set; }

        public DateTime isActive { get; set; }
    }
}