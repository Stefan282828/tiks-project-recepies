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
        public DbSet<ReceptSastojak> ReceptSastojci { get; set; }
        public DbSet<Sastojak> Sastojci { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recept>()
                .HasOne(r => r.Podkategorija)
                .WithMany(pk => pk.Recepti)
                .HasForeignKey(r => r.PodKategorijaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Podkategorija>()
                .HasOne<Kategorija>()
                .WithMany(k => k.Podkategorije)
                .HasForeignKey("KategorijaId") 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReceptSastojak>()
                .HasKey(rs => new { rs.ReceptId, rs.SastojakId }); 

            modelBuilder.Entity<ReceptSastojak>()
                .HasOne(rs => rs.Recept)
                .WithMany(r => r.ReceptSastojci)
                .HasForeignKey(rs => rs.ReceptId);

            modelBuilder.Entity<ReceptSastojak>()
                .HasOne(rs => rs.Sastojak)
                .WithMany(s => s.Recepti)
                .HasForeignKey(rs => rs.SastojakId);
        }
    }
}
