using System;

namespace FoodExplorer.Models
{
    public class Recept
    {
        //public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int VremePripreme { get; set; }
        public string Kategorija { get; set; }
        //public List<string> Sastojci { get; set; }
        public string UputstvoPripreme { get; set; }
}


}
