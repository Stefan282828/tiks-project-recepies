using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodExplorer.Models;
using FoodExplorer.Modules;
using Microsoft.Extensions.Logging;

namespace FoodExplorer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KategorijaController : ControllerBase
    {
        private readonly IGraphClient _neo4JClient;
        private static ILogger<KategorijaController> _logger;
        private static KategorijaModule _modules;
        public KategorijaController(IGraphClient neo4JClient, ILogger<KategorijaController> logger)
        {
             _modules = new KategorijaModule(neo4JClient, logger );
            _neo4JClient = neo4JClient;
            _logger = logger;
        }

        [HttpPost("DodajKategoriju")]
        public async Task<IActionResult> DodajKategoriju([FromBody] Kategorija novakategorija)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                await _neo4JClient.Cypher
                    .Create("(k:Kategorija $kategorijaParam)")
                    .WithParam("kategorijaParam", novakategorija)
                    .ExecuteWithoutResultsAsync();

                return Ok("Kategorija uspešno dodata.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dodavanja kategorije: {ex.Message}");
            }
        }

 
        [HttpGet("VratiKategorije")]
        public async Task<IActionResult> VratiKategorije()
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var kategorije = await _neo4JClient.Cypher
                    .Match("(kategorija:Kategorija)")
                    .Return(kategorija => kategorija.As<Kategorija>())
                    .ResultsAsync;

                return Ok(kategorije);
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja kategorije: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("VratiKategorijePoIdu/{id}")]
        public async Task<IActionResult> KategorijaPoId(int id)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var kategorija = await _neo4JClient.Cypher
                    .Match("(kategorija:Kategorija)")
                    .Where((Kategorija kategorija) => kategorija.Id == id)
                    .Return(kategorija => kategorija.As<Kategorija>())
                    .ResultsAsync;

                return Ok(kategorija.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja kategorije: {ex.Message}");
            }
        }
        [HttpDelete]
        [Route("ObrisiKategoriju/{naziv}")]
        public async Task<IActionResult> Delete(string naziv)
        {
            await  _neo4JClient.Cypher.Match("(r:Kategorija)")
                                 .Where((Kategorija r) => r.Naziv == naziv)
                                 .Delete("r")
                                 .ExecuteWithoutResultsAsync();
            return Ok(new { Message = "Kategorija uspešno obrisana." });

        }

        [HttpPut]
        [Route("AzurirajKategoriju/{naziv}")]
        public async Task<IActionResult> AzurirajKategoriju(string naziv, [FromBody] Kategorija azuriranaKategorija)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var query = await _neo4JClient.Cypher
                    .Match("(kategorija:Kategorija)")
                    .Where((Kategorija kategorija) => kategorija.Naziv == naziv)
                    .Set("kategorija = $azuriranaKategorija")
                    .WithParam("azuriranaKategorija", azuriranaKategorija)
                    .Return(kategorija => kategorija.As<Kategorija>())
                    .ResultsAsync;

                var azurirana1Kategorija = query.SingleOrDefault();

                if (azurirana1Kategorija == null)
                {
                    return NotFound($"Recept sa nazivom {naziv} nije pronađen.");
                }

                return Ok(azurirana1Kategorija);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Greška prilikom ažuriranja kategorije: {ex.Message}");
                return BadRequest($"Greška prilikom ažuriranja kategorije: {ex.Message}");
            }
        }

        [HttpPost("dodaj-podkategoriju-kategoriji/{podkategorijaId}/{kategorijaId}")]

        public async Task<ActionResult> CreateRelationship(int podkategorijaId, int kategorijaId)
        {
            return Ok(_modules.AddPodKategorija(podkategorijaId,kategorijaId));
        }
    }
}
