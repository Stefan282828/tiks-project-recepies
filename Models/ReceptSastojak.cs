using System.ComponentModel.DataAnnotations.Schema;
using FoodExplorer.Models;

public class ReceptSastojak
{
    public int ReceptId { get; set; }
    public Recept? Recept { get; set; }

    public int SastojakId { get; set; }
    public Sastojak? Sastojak { get; set; }

    [Column(TypeName = "decimal(5,2)")]  
    public decimal Kolicina { get; set; }
    
    public string JedinicaMere { get; set; } = string.Empty;
}
