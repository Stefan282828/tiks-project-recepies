using System;
using System.ComponentModel.DataAnnotations;

namespace FoodExplorer.Models
{
    public class Podkategorija
    {
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; } = string.Empty;

        // FK to Kategorija
        public int KategorijaId { get; set; }
        public virtual Kategorija Kategorija { get; set; } = null!;

        public virtual ICollection<Recept> Recepti { get; set; } = new List<Recept>();
    }
}
