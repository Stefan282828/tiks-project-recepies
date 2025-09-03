using FoodExplorer.Data;
using FoodExplorer.Models;
using FoodExplorer.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FoodExplorer.Services
{
    public class PodkategorijaService : IPodkategorijaService
    {
        private readonly FoodExplorerContext _context;

        public PodkategorijaService(FoodExplorerContext context)
        {
            _context = context;
        }

        public async Task<Podkategorija> CreatePodkategorijaAsync(PodkategorijaRequestDto dto)
        {
            var podkategorija = new Podkategorija
            {
                Naziv = dto.Naziv
            };

            // assign shadow FK KategorijaId
            _context.Entry(podkategorija).Property("KategorijaId").CurrentValue = dto.KategorijaId;

            _context.Podkategorije.Add(podkategorija);
            await _context.SaveChangesAsync();

            return podkategorija;
        }

        public async Task<IEnumerable<Podkategorija>> GetAllPodkategorijeAsync()
        {
            return await _context.Podkategorije
                                 .Include(p => p.Recepti)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Podkategorija>> GetByKategorijaIdAsync(int kategorijaId)
        {
            return await _context.Podkategorije
                                 .Where(p => EF.Property<int?>(p, "KategorijaId") == kategorijaId)
                                 .Include(p => p.Recepti)
                                 .ToListAsync();
        }

        public async Task<Podkategorija?> GetIdAsync(int id)
        {
            return await _context.Podkategorije
                                 .Include(p => p.Recepti)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Podkategorija> UpdatePodkategorijaAsync(int id, PodkategorijaRequestDto dto)
        {
            var existing = await _context.Podkategorije.FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null) return null;

            existing.Naziv = dto.Naziv;
            _context.Entry(existing).Property("KategorijaId").CurrentValue = dto.KategorijaId;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeletePodkategorijaAsync(int id)
        {
            var podkategorija = await _context.Podkategorije.FirstOrDefaultAsync(p => p.Id == id);
            if (podkategorija == null) return false;

            _context.Podkategorije.Remove(podkategorija);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
