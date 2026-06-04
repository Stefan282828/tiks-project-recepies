namespace FoodExplorer.Models.Dto
{
    public class ReceptUpdateDto
    {
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public int VremePripreme { get; set; }
        public string UputstvoPripreme { get; set; } = string.Empty;
        public int PodKategorijaId { get; set; }
    }
}
