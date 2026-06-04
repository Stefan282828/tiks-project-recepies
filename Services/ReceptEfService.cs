using FoodExplorer.Data;
using FoodExplorer.Models;
using FoodExplorer.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FoodExplorer.Services
{
    public class ReceptEfService : IReceptService
    {
        private readonly FoodExplorerContext _context;
        public ReceptEfService(FoodExplorerContext context) { _context = context; }

        public async Task<Recept> CreateAsync(ReceptCreateDto dto)
        {
            var recept = new Recept
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                VremePripreme = dto.VremePripreme,
                UputstvoPripreme = dto.UputstvoPripreme,
                PodKategorijaId = dto.PodKategorijaId
            };
            _context.Recepti.Add(recept);
            await _context.SaveChangesAsync();
            return recept;
        }

        public async Task<IEnumerable<Recept>> GetAllAsync()
        {
            return await _context.Recepti
                .Include(r => r.Podkategorija)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recept>> GetByPodkategorijaAsync(int podkategorijaId)
        {
            return await _context.Recepti
                .Where(r => r.PodKategorijaId == podkategorijaId)
                .Include(r => r.Podkategorija)
                .ToListAsync();
        }

        public async Task<Recept?> GetByIdAsync(int id)
        {
            return await _context.Recepti
                .Include(r => r.Podkategorija)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recept?> UpdateAsync(int id, ReceptUpdateDto dto)
        {
            var existing = await _context.Recepti.FirstOrDefaultAsync(r => r.Id == id);
            if (existing == null) return null;
            existing.Naziv = dto.Naziv;
            existing.Opis = dto.Opis;
            existing.VremePripreme = dto.VremePripreme;
            existing.UputstvoPripreme = dto.UputstvoPripreme;
            existing.PodKategorijaId = dto.PodKategorijaId;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var recept = await _context.Recepti.FirstOrDefaultAsync(r => r.Id == id);
            if (recept == null) return false;
            _context.Recepti.Remove(recept);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
