using System.Threading.Tasks;
using NUnit.Framework;
using FoodExplorer.Data;
using Microsoft.EntityFrameworkCore;

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

        var created = await service.CreateKategorijaAsync(new FoodExplorer.Models.Dto.KategorijaCreateDto { Naziv = "Italijanska" });

        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.Naziv, Is.EqualTo("Italijanska"));
        Assert.That(await ctx.Kategorije.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateKategorija_ShouldChangeName()
    {
        using var ctx = CreateContext();
        var service = new KategorijaService(ctx);
        var created = await service.CreateKategorijaAsync(new FoodExplorer.Models.Dto.KategorijaCreateDto { Naziv = "Staro" });

        var updated = await service.UpdateKategorijaAsync(created.Id, new FoodExplorer.Models.Dto.KategorijaUpdateDto { Naziv = "Novo" });

        Assert.IsNotNull(updated);
        Assert.That(updated!.Naziv, Is.EqualTo("Novo"));
    }

    [Test]
    public async Task DeleteKategorija_ShouldRemove()
    {
        using var ctx = CreateContext();
        var service = new KategorijaService(ctx);
        var created = await service.CreateKategorijaAsync(new FoodExplorer.Models.Dto.KategorijaCreateDto { Naziv = "ZaBrisanje" });

        var ok = await service.DeleteKategorijaAsync(created.Id);

        Assert.IsTrue(ok);
        Assert.That(await ctx.Kategorije.CountAsync(), Is.EqualTo(0));
    }
}
