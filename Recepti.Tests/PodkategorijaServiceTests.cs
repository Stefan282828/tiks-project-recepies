using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using FoodExplorer.Data;
using FoodExplorer.Services;
using FoodExplorer.Models.Dto;

namespace Recepti.Tests
{
    [TestFixture]
    public class PodkategorijaServiceTests
    {
        private FoodExplorerContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<FoodExplorerContext>()
                .UseInMemoryDatabase(databaseName: $"podkategorija_db_{System.Guid.NewGuid()}")
                .Options;
            return new FoodExplorerContext(options);
        }

        [Test]
        public async Task CreatePodkategorija_WithKategorijaId_ShouldPersistAndLink()
        {
            using var ctx = CreateContext();
            var katService = new KategorijaService(ctx);
            var podService = new PodkategorijaService(ctx);
            var kat = await katService.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Italijanska" });

            var pod = await podService.CreatePodkategorijaAsync(new PodkategorijaRequestDto { Naziv = "Pasta", KategorijaId = kat.Id });

            Assert.That(pod.Id, Is.GreaterThan(0));
            Assert.That(pod.Naziv, Is.EqualTo("Pasta"));
            Assert.That(pod.KategorijaId, Is.EqualTo(kat.Id));
            Assert.That(await ctx.Podkategorije.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByKategorijaId_ShouldReturnOnlyMatching()
        {
            using var ctx = CreateContext();
            var katService = new KategorijaService(ctx);
            var podService = new PodkategorijaService(ctx);
            var kat1 = await katService.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Italijanska" });
            var kat2 = await katService.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Meksička" });

            await podService.CreatePodkategorijaAsync(new PodkategorijaRequestDto { Naziv = "Pasta", KategorijaId = kat1.Id });
            await podService.CreatePodkategorijaAsync(new PodkategorijaRequestDto { Naziv = "Tacos", KategorijaId = kat2.Id });

            var res = await podService.GetByKategorijaIdAsync(kat1.Id);
            Assert.That(res.Count(), Is.EqualTo(1));
            Assert.That(res.First().Naziv, Is.EqualTo("Pasta"));
        }

        [Test]
        public async Task UpdatePodkategorija_ShouldChangeName()
        {
            using var ctx = CreateContext();
            var katService = new KategorijaService(ctx);
            var podService = new PodkategorijaService(ctx);
            var kat = await katService.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Italijanska" });
            var pod = await podService.CreatePodkategorijaAsync(new PodkategorijaRequestDto { Naziv = "Staro", KategorijaId = kat.Id });

            var updated = await podService.UpdatePodkategorijaAsync(pod.Id, new PodkategorijaRequestDto { Naziv = "Novo", KategorijaId = kat.Id });

            Assert.IsNotNull(updated);
            Assert.That(updated!.Naziv, Is.EqualTo("Novo"));
        }

        [Test]
        public async Task DeletePodkategorija_ShouldRemove()
        {
            using var ctx = CreateContext();
            var katService = new KategorijaService(ctx);
            var podService = new PodkategorijaService(ctx);
            var kat = await katService.CreateKategorijaAsync(new KategorijaCreateDto { Naziv = "Italijanska" });
            var pod = await podService.CreatePodkategorijaAsync(new PodkategorijaRequestDto { Naziv = "Pasta", KategorijaId = kat.Id });

            var ok = await podService.DeletePodkategorijaAsync(pod.Id);

            Assert.IsTrue(ok);
            Assert.That(await ctx.Podkategorije.CountAsync(), Is.EqualTo(0));
        }
    }
}
