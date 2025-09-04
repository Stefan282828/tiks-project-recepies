using FoodExplorer.Data;
using FoodExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodExplorer.Models.Dto;



public class KategorijaService : IKategorijaService
{
    private readonly FoodExplorerContext _context;

    public KategorijaService(FoodExplorerContext context)
    {
        _context = context;
    }

    public async Task<Kategorija> CreateKategorijaAsync(KategorijaCreateDto dto)
    {
        var kategorija = new Kategorija
        {
            Naziv = dto.Naziv
        };

        _context.Kategorije.Add(kategorija);
        await _context.SaveChangesAsync();

        return kategorija;
    }

    public async Task<IEnumerable<Kategorija>> GetAllKategorijeAsync()
    {
        return await _context.Kategorije
                             .Include(x => x.Podkategorije)
                             .ToListAsync();
    }

    public async Task<Kategorija> GetIdAsync(int id)
    {
        return await _context.Kategorije
                             .Include(x => x.Podkategorije)
                             .FirstOrDefaultAsync(k => k.Id == id);
                    
    }

    public async Task<Kategorija> UpdateKategorijaAsync(int id, KategorijaUpdateDto dto)
    {
        var existing = await _context.Kategorije.FirstOrDefaultAsync(k => k.Id == id);
        if (existing == null) return null;

        existing.Naziv = dto.Naziv;  
        
        await _context.SaveChangesAsync();
        return existing;
    }
    public async Task<bool> DeleteKategorijaAsync(int id)
    {
        var kategorija = await _context.Kategorije.FirstOrDefaultAsync(k => k.Id == id);
        if (kategorija == null) return false;

        _context.Kategorije.Remove(kategorija);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task AddPodkategorijaAsync(int podkategorijaId, int kategorijaId)
    {
        var kategorija = await _context.Kategorije.FirstOrDefaultAsync(k => k.Id == kategorijaId);
        if (kategorija == null) throw new KeyNotFoundException("Kategorija nije pronađena");

        var pod = await _context.Podkategorije.FirstOrDefaultAsync(p => p.Id == podkategorijaId);
        if (pod == null) throw new KeyNotFoundException("Podkategorija nije pronađena");

        pod.KategorijaId = kategorijaId;
        await _context.SaveChangesAsync();
    }

}
