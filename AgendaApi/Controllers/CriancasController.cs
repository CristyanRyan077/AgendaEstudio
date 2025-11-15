using AgendaShared.DTOs;
using AgendaApi.Infra.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CriancasController : ControllerBase
    {
        private readonly ICriancaService _service;
        public CriancasController(ICriancaService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CriancaDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CriancaDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }
        [HttpGet("{id:int}/by-clienteId")]
        public async Task<ActionResult<CriancaDto>> GetByClienteId(int id)
        {
            var item = await _service.GetByClienteIdAsync(id);
            return Ok(item);
        }

        [HttpPost("by-cliente/{clienteId}")]
        public async Task<ActionResult<CriancaDto>> Create(int clienteId, [FromBody] CriancaCreateDto dto)
        {
            var novo = await _service.CreateAsync(clienteId, dto);
            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<CriancaDto>> Update(int id, CriancaUpdateDto dto)
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