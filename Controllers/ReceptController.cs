using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using FoodExplorer.Models;
using FoodExplorer.Modules;



namespace FoodExplorer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReceptController : ControllerBase
    {
        private static IGraphClient _neo4JClient;

        private static ILogger<ReceptController> _logger;


         private static ReceptModule _modules;


        public ReceptController(IGraphClient neo4JClient,  ILogger<ReceptController> logger)
        {
             _modules = new ReceptModule(neo4JClient, logger );
            _neo4JClient = neo4JClient;
            _logger = logger;
            
        }

        [HttpPost("DodajRecept")]
        public async Task<IActionResult> DodajRecept([FromBody] Recept novrecept)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                await _neo4JClient.Cypher
                    .Create("(k:Recept $receptParam)")
                    .WithParam("receptParam", novrecept)
                    .ExecuteWithoutResultsAsync();

                return Ok("Recept uspešno dodat.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dodavanja recepta: {ex.Message}");
            }
        }

        
        [HttpGet("VratiRecepte")]
        public async Task<IActionResult> VratiRecepte()
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var recepti = await _neo4JClient.Cypher
                    .Match("(recept:Recept)")
                    .Return(recept => recept.As<Recept>())
                    .ResultsAsync;

                return Ok(recepti);
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja podkategorija: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("VratiReceptPoNazivu/{name}")]
        public async Task<IActionResult> ReceptPoNazivu(string name)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var recept = await _neo4JClient.Cypher
                    .Match("(recept:Recept)")
                    .Where((Recept recept) => recept.Naziv == name)
                    .Return(recept => recept.As<Recept>())
                    .ResultsAsync;

                return Ok(recept.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška prilikom dohvatanja recepta: {ex.Message}");
            }
        }

        [HttpDelete("ObrisiRecept/{naziv}")]
        public async Task<IActionResult> Delete(string naziv)
        {
            await  _neo4JClient.Cypher.Match("(r:Recept)")
                                 .Where((Recept r) => r.Naziv == naziv)
                                 .Delete("r")
                                 .ExecuteWithoutResultsAsync();
            return Ok(new { Message = "Recept uspešno obrisan." });

        }

        [HttpPut("UpdateRecept/{naziv}")]
        public async Task<IActionResult> AzurirajRecept(string naziv, [FromBody] Recept azuriranRecept)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var query = await _neo4JClient.Cypher
                    .Match("(recept:Recept)")
                    .Where((Recept recept) => recept.Naziv == naziv)
                    .Set("recept = $azuriranRecept")
                    .WithParam("azuriranRecept", azuriranRecept)
                    .Return(recept => recept.As<Recept>())
                    .ResultsAsync;

                var azuriraniRecept = query.SingleOrDefault();

                if (azuriraniRecept == null)
                {
                    return NotFound($"Recept sa nazivom {naziv} nije pronađen.");
                }

                return Ok(azuriraniRecept);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Greška prilikom ažuriranja recepta: {ex.Message}");
                return BadRequest($"Greška prilikom ažuriranja recepta: {ex.Message}");
            }
        }

        [HttpPost("dodaj-sastojak-receptu/{receptNaziv}/{sastojakId}")]

        public async Task<ActionResult> CreateRelationship(string receptNaziv, int sastojakId)
        {
            return Ok(_modules.AddSastojak(receptNaziv,sastojakId));
        }

        [HttpPost("dodaj-podkategoriju-receptu/{receptId}/{podkategorijaId}")]

        public async Task<ActionResult>  CreateRel(int receptId, int podkategorijaId)
        {
            return Ok(_modules.AddPodkategorija(receptId,podkategorijaId));
        }



    }


    
}
