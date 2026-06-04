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
    public class SastojakController : ControllerBase
    {
        private readonly IGraphClient _neo4JClient;

        private static ILogger<SastojakController> _logger;
    

        public SastojakController(IGraphClient neo4JClient, ILogger<SastojakController> logger)
        {
            _neo4JClient = neo4JClient;
           _logger = logger;
        }

        [HttpPost("DodajSastojak")]
        public async Task<IActionResult> DodajSastojak([FromBody] Sastojak novsastojak)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                await _neo4JClient.Cypher
                    .Create("(k:Sastojak $sastojakParam)")
                    .WithParam("sastojakParam", novsastojak)
                    .ExecuteWithoutResultsAsync();

                return Ok("Sastojak uspešno dodata.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dodavanja sastojka: {ex.Message}");
            }
        }

        [HttpGet("VratiSastojke")]
        public async Task<IActionResult> VratiSastojke()
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var sastojci = await _neo4JClient.Cypher
                    .Match("(sastojak:Sastojak)")
                    .Return(sastojak => sastojak.As<Sastojak>())
                    .ResultsAsync;

                return Ok(sastojci);
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja sastojaka: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("VratiSastojakPoIdu/{id}")]
        public async Task<IActionResult> VratiSastojakPoIdu(int id)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var sastojak = await _neo4JClient.Cypher
                    .Match("(sastojak:Sastojak)")
                    .Where((Sastojak sastojak) => sastojak.Id == id)
                    .Return(sastojak => sastojak.As<Sastojak>())
                    .ResultsAsync;

                return Ok(sastojak.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja sastojka: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("VratiSastojakPoNazivu/{naziv}")]
        public async Task<IActionResult> VratiSastojakPoNazivu(string naziv)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var sastojak = await _neo4JClient.Cypher
                    .Match("(sastojak:Sastojak)")
                    .Where((Sastojak sastojak) => sastojak.Naziv == naziv)
                    .Return(sastojak => sastojak.As<Sastojak>())
                    .ResultsAsync;

                return Ok(sastojak.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja sastojka: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("ObrisiSastojak/{naziv}")]
        public async Task<IActionResult> Delete(string naziv)
        {
            await  _neo4JClient.Cypher.Match("(r:Sastojak)")
                                 .Where((Sastojak r) => r.Naziv == naziv)
                                 .Delete("r")
                                 .ExecuteWithoutResultsAsync();
            return Ok(new { Message = "Sastojak uspešno obrisana." });

        }


        [HttpPut]
        [Route("AzurirajSastojak/{naziv}")]
        public async Task<IActionResult> AzurirajSastojak(string naziv, [FromBody] Sastojak azuriranSastojak)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var query = await _neo4JClient.Cypher
                    .Match("(sastojak:Sastojak)")
                    .Where((Sastojak sastojak) => sastojak.Naziv == naziv)
                    .Set("sastojak = $azuriranSastojak")
                    .WithParam("azuriranSastojak", azuriranSastojak)
                    .Return(sastojak => sastojak.As<Sastojak>())
                    .ResultsAsync;

                var azuriraniSastojak = query.SingleOrDefault();

                if (azuriraniSastojak == null)
                {
                    return NotFound($"Sastojak sa nazivom {naziv} nije pronađen.");
                }

                return Ok(azuriraniSastojak);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Greška prilikom ažuriranja kategorije: {ex.Message}");
                return BadRequest($"Greška prilikom ažuriranja kategorije: {ex.Message}");
            }
        }
    }
}
