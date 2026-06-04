using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodExplorer.Models;
using FoodExplorer.Models.Dto;

namespace FoodExplorer.Controllers
{
    [ApiController]
    [Route("Kategorija")]
    public class KategorijaController : ControllerBase
    {
        private readonly IKategorijaService _service;

        public KategorijaController(IKategorijaService service)
        {
            _service = service;
        }

        [HttpPost("Dodaj")]
        public async Task<ActionResult<Kategorija>> Create([FromBody] KategorijaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var k = await _service.CreateKategorijaAsync(dto);
            return Ok(k);
        }

        [HttpGet("VratiSve")]
        public async Task<ActionResult<IEnumerable<Kategorija>>> GetAll()
        {
            var kategorije = await _service.GetAllKategorijeAsync();
            return Ok(kategorije);
        }
  
        [HttpGet("{id}")]
        public async Task<ActionResult<Kategorija>> GetById(int id)
        {
            var kategorija = await _service.GetIdAsync(id);
            if (kategorija == null) return NotFound();
            return Ok(kategorija);
        }

        [HttpPut("Izmeni/{id}")]
        public async Task<ActionResult<Kategorija>> Update(int id, [FromBody] KategorijaUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateKategorijaAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("Obrisi/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteKategorijaAsync(id);
            if (!deleted) return NotFound();
            return Ok("Kategorija obrisana!");
        }
    }
}
