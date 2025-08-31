using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodExplorer.Models
{
public class Sastojak
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Naziv { get; set; } = string.Empty; 

    [Required]  
    public string JedinicaMere { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(5,2)")]  
    public decimal Kolicina { get; set; } 

    public List<ReceptSastojak>? Recepti { get; set; }
}


}