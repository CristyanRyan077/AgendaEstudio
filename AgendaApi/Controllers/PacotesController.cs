using AgendaApi.Domain.DTOs;
using AgendaApi.Infra.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacotesController : ControllerBase
    {
        private readonly IPacoteService _service;
        public PacotesController(IPacoteService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacoteDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PacoteDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<PacoteDto>> Create(PacoteCreateDto dto)
        {
            var novo = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PacoteDto>> Update(int id, PacoteUpdateDto dto)
        {
            var atualizado = await _service.UpdateAsync(id, dto);
            return Ok(atualizado);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}