using FoodExplorer.Models;
using FoodExplorer.Models.Dto;

namespace FoodExplorer.Services
{
    public interface IReceptService
    {
        Task<Recept> CreateAsync(ReceptCreateDto dto);
        Task<IEnumerable<Recept>> GetAllAsync();
        Task<IEnumerable<Recept>> GetByPodkategorijaAsync(int podkategorijaId);
        Task<Recept?> GetByIdAsync(int id);
        Task<Recept?> UpdateAsync(int id, ReceptUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
