using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodExplorer.Models
{
    public class Recept
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Naziv { get; set; } = string.Empty;

        public string Opis { get; set; } = string.Empty;

        public int VremePripreme { get; set; }

        public string UputstvoPripreme { get; set; } = string.Empty;

        [Required]
        public int PodKategorijaId { get; set; }

        [ForeignKey("PodKategorijaId")]
        public virtual Podkategorija? Podkategorija { get; set; }

        public List<ReceptSastojak>? ReceptSastojci { get; set; }
}


}
