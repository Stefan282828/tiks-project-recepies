using System;
using System.ComponentModel.DataAnnotations;

namespace  FoodExplorer.Models
{
    public class Kategorija
    {
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; } = string.Empty;

        public virtual ICollection<Podkategorija> Podkategorije { get; set; } = new List<Podkategorija>();
        
        
    }
 
}