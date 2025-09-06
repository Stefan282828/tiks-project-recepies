using FoodExplorer.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodExplorer.Data
{
    public class FoodExplorerContext : DbContext
    {
        public FoodExplorerContext(DbContextOptions<FoodExplorerContext> options) : base(options)
        {
        }

        public DbSet<Kategorija> Kategorije { get; set; }
        public DbSet<Podkategorija> Podkategorije { get; set; }
        public DbSet<Recept> Recepti { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recept>()
                .HasOne(r => r.Podkategorija)
                .WithMany(pk => pk.Recepti)
                .HasForeignKey(r => r.PodKategorijaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Podkategorija>()
                .HasOne(p => p.Kategorija)
                .WithMany(k => k.Podkategorije)
                .HasForeignKey(p => p.KategorijaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
