using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using FoodExplorer.Data;
using FoodExplorer.Services;
using FoodExplorer.Models.Dto;
using System.ComponentModel.DataAnnotations;

namespace Recepti.Tests
{
    [TestFixture]
    public class KategorijaServiceTests
    {
        private FoodExplorerContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<FoodExplorerContext>()
                .UseInMemoryDatabase(databaseName: $"kategorija_db_{System.Guid.NewGuid()}")
                .Options;
            return new FoodExplorerContext(options);
        }

        [Test]
        public async Task CreateKategorija_ShouldAssignId_AndPersist()
        {
            using var ctx = CreateContext();
            var service = new KategorijaService(ctx);

            var created = await service.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Italijanska" });

            Assert.That(created.Id, Is.GreaterThan(0));
            Assert.That(created.Naziv, Is.EqualTo("Italijanska"));
            Assert.That(await ctx.Kategorije.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateKategorija_ShouldChangeName()
        {
            using var ctx = CreateContext();
            var service = new KategorijaService(ctx);
            var created = await service.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Staro" });

            var updated = await service.UpdateKategorijaAsync(created.Id, new KategorijaUpdateDto { Naziv = "Novo" });

            Assert.IsNotNull(updated);
            Assert.That(updated!.Naziv, Is.EqualTo("Novo"));
        }

        [Test]
        public void UpdateKategorija_NonExistingId_ShouldThrowException()
        {
            using var ctx = CreateContext();
            var service = new KategorijaService(ctx);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await service.UpdateKategorijaAsync(9999, new KategorijaUpdateDto { Naziv = "Novo" })
            );
        }
        [Test]
        public void UpdateKategorija_EmptyName_ShouldFailValidation()
        {
            using var ctx = CreateContext();
            var service = new KategorijaService(ctx);
            var created = service.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Staro" }).Result;

            Assert.ThrowsAsync<ValidationException>(async () =>
                await service.UpdateKategorijaAsync(created.Id, new KategorijaUpdateDto { Naziv = "" })
            );
        }


        [Test]
        public async Task DeleteKategorija_ShouldRemove()
        {
            using var ctx = CreateContext();
            var service = new KategorijaService(ctx);
            var created = await service.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "ZaBrisanje" });

            var ok = await service.DeleteKategorijaAsync(created.Id);

            Assert.IsTrue(ok);
            Assert.That(await ctx.Kategorije.CountAsync(), Is.EqualTo(0));
        }
    }
}
