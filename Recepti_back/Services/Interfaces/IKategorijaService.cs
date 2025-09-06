using FoodExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using FoodExplorer.Models.Dto;

public interface IKategorijaService
{
    Task<Kategorija> CreateKategorijaAsync(KategorijaCreateDto dto);
    Task<IEnumerable<Kategorija>> GetAllKategorijeAsync();
    Task<Kategorija?> GetIdAsync(int id);
    Task<Kategorija> UpdateKategorijaAsync(int id, KategorijaUpdateDto dto);
    Task<bool> DeleteKategorijaAsync(int id);
    Task AddPodkategorijaAsync(int podkategorijaId, int kategorijaId);  
}