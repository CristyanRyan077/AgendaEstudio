using AgendaApi.Domain.DTOs;
using AgendaApi.Infra.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicosController : ControllerBase
    {
        private readonly IServicoService _service;
        public ServicosController(IServicoService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicoDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServicoDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ServicoDto>> Create(ServicoCreateDto dto)
        {
            var novo = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ServicoDto>> Update(int id, ServicoUpdateDto dto)
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