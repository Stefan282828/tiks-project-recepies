using FoodExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using FoodExplorer.Models.Dto;

namespace FoodExplorer.Services
{
    public interface IPodkategorijaService
    {
        Task<Podkategorija> CreatePodkategorijaAsync(PodkategorijaRequestDto dto);
        Task<IEnumerable<Podkategorija>> GetAllPodkategorijeAsync();
        Task<Podkategorija> GetIdAsync(int id);
        Task<Podkategorija> UpdatePodkategorijaAsync(int id, PodkategorijaRequestDto dto);
        Task<bool> DeletePodkategorijaAsync(int id);
    }
}