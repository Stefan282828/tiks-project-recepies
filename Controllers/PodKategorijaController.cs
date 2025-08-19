using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodExplorer.Models;
namespace FoodExplorer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PodkategorijaController : ControllerBase
    {
        private readonly IGraphClient _neo4JClient;

        private readonly ILogger<PodkategorijaController> _logger;

        public PodkategorijaController(IGraphClient neo4JClient, ILogger<PodkategorijaController> logger)
        {
            _neo4JClient = neo4JClient;
        }

        [HttpPost("DodajPodkategoriju")]
        public async Task<IActionResult> DodajPodKategoriju([FromBody] Podkategorija novapodkategorija)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                await _neo4JClient.Cypher
                    .Create("(k:Podkategorija $podkategorijaParam)")
                    .WithParam("podkategorijaParam", novapodkategorija)
                    .ExecuteWithoutResultsAsync();

                return Ok("Podkategorija uspešno dodata.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dodavanja podkategorije: {ex.Message}");
            }
        }
        [HttpGet("VratiPodkategorije")]
        public async Task<IActionResult> VratiPodkategorije()
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var podkategorije = await _neo4JClient.Cypher
                    .Match("(podkategorija:Podkategorija)")
                    .Return(podkategorija => podkategorija.As<Podkategorija>())
                    .ResultsAsync;

                return Ok(podkategorije);
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja podkategorija: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("VratiPodkategorijePoNazivu/{name}")]
        public async Task<IActionResult> PodkategorijaPoNazivu(string name)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var recept = await _neo4JClient.Cypher
                    .Match("(podkategorija:Podkategorija)")
                    .Where((Podkategorija podkategorija) => podkategorija.Naziv == name)
                    .Return(podkategorija => podkategorija.As<Podkategorija>())
                    .ResultsAsync;

                return Ok(recept.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja podkategorije: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("VratiPodkategorijePoIdu/{id}")]
        public async Task<IActionResult> PodkategorijaPoIdu(int id)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var recept = await _neo4JClient.Cypher
                    .Match("(podkategorija:Podkategorija)")
                    .Where((Podkategorija podkategorija) => podkategorija.Id == id)
                    .Return(podkategorija => podkategorija.As<Podkategorija>())
                    .ResultsAsync;

                return Ok(recept.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja podkategorije: {ex.Message}");
            }
        }


        [HttpDelete("{naziv}")]
        public async Task<IActionResult> Delete(string naziv)
        {
            await  _neo4JClient.Cypher.Match("(r:Podkategorija)")
                                 .Where((Podkategorija r) => r.Naziv == naziv)
                                 .Delete("r")
                                 .ExecuteWithoutResultsAsync();
            return Ok(new { Message = "Podkategorija uspešno obrisana." });

        }

        [HttpPut("{naziv}")]
        public async Task<IActionResult> AzurirajPodkategoriju(string naziv, [FromBody] Podkategorija azurirano)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var query = await _neo4JClient.Cypher
                    .Match("(podkategorija:Podkategorija)")
                    .Where((Podkategorija podkategorija) => podkategorija.Naziv == naziv)
                    .Set("podkategorija = $azurirano")
                    .WithParam("azurirano", azurirano)
                    .Return(podkategorija => podkategorija.As<Podkategorija>())
                    .ResultsAsync;

                var azuriranje = query.SingleOrDefault();

                if (azuriranje == null)
                {
                    return NotFound($"Podkategorija sa ID {naziv} nije pronađena.");
                }

                return Ok(azuriranje);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Greška prilikom ažuriranja recepta: {ex.Message}");
                return BadRequest($"Greška prilikom ažuriranja recepta: {ex.Message}");
            }
        }
    }
}
