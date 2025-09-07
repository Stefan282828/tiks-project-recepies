using Microsoft.AspNetCore.Mvc;
using FoodExplorer.Models;
using FoodExplorer.Models.Dto;
using FoodExplorer.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Controllers
{
    [ApiController]
    [Route("Recept")]
    public class ReceptController : ControllerBase
    {
        private readonly IReceptService _service;
        public ReceptController(IReceptService service) { _service = service; }

        [HttpPost("Dodaj")]
        public async Task<ActionResult<Recept>> Create([FromBody] ReceptCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpGet("VratiSve")]
        public async Task<ActionResult<IEnumerable<Recept>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("ZaPodkategoriju/{podkategorijaId}")]
        public async Task<ActionResult<IEnumerable<Recept>>> GetByPodkategorija(int podkategorijaId)
        {
            var items = await _service.GetByPodkategorijaAsync(podkategorijaId);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recept>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPut("Izmeni/{id}")]
        public async Task<ActionResult<Recept>> Update(int id, [FromBody] ReceptUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("Obrisi/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok("Recept obrisan!");
        }
    }
}
