using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodExplorer.Models;
using FoodExplorer.Models.Dto;
using FoodExplorer.Services;


namespace FoodExplorer.Controllers
{
    [ApiController]
    [Route("Podkategorija")]
    public class PodkategorijaController : ControllerBase
    {
        private readonly IPodkategorijaService _service;

        public PodkategorijaController(IPodkategorijaService service)
        {
            _service = service;
        }

        [HttpPost("Dodaj")]
        public async Task<ActionResult<Podkategorija>> Create([FromBody] PodkategorijaRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var p = await _service.CreatePodkategorijaAsync(dto);
            return Ok(p);
        }

        [HttpGet("VratiSve")]
        public async Task<ActionResult<IEnumerable<Podkategorija>>> GetAll()
        {
            var podkategorije = await _service.GetAllPodkategorijeAsync();
            return Ok(podkategorije);
        }

        [HttpGet("ZaKategoriju/{kategorijaId}")]
        public async Task<ActionResult<IEnumerable<Podkategorija>>> GetByKategorija(int kategorijaId)
        {
            var podkategorije = await _service.GetByKategorijaIdAsync(kategorijaId);
            return Ok(podkategorije);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Podkategorija>> GetById(int id)
        {
            var podkategorija = await _service.GetIdAsync(id);
            if (podkategorija == null) return NotFound();
            return Ok(podkategorija);
        }

        [HttpPut("Izmeni/{id}")]
        public async Task<ActionResult<Podkategorija>> Update(int id, [FromBody] PodkategorijaRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdatePodkategorijaAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("Obrisi/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeletePodkategorijaAsync(id);
            if (!deleted) return NotFound();
            return Ok("Podkategorija obrisana!");
        }
    }
}
