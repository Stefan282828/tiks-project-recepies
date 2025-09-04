using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FoodExplorer.Models
{
    public class Podkategorija
    {
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; } = string.Empty;

        // FK to Kategorija
        public int KategorijaId { get; set; }

        [JsonIgnore]
        public virtual Kategorija Kategorija { get; set; } = null!;

        public virtual ICollection<Recept> Recepti { get; set; } = new List<Recept>();
    }
}
