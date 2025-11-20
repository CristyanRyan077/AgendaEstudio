using AgendaApi.Application.Services;
using AgendaShared;
using AgendaShared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LembretesController : ControllerBase
    {
        private readonly ILembreteService _service;
        public LembretesController(ILembreteService service) => _service = service;

        // GET api/lembretes/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LembreteDto>> GetById(int id, CancellationToken ct)
        {
            var lembrete = await _service.GetByIdAsync(id, ct);
            if (lembrete is null) return NotFound();
            return Ok(lembrete);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LembreteDto>>> List([FromQuery] LembreteQuery filtro, CancellationToken ct)
        {
            Console.WriteLine($"[Controller] Inicio={filtro.Inicio}, Fim={filtro.Fim}, ClienteId={filtro.ClienteId}, AgendamentoId={filtro.AgendamentoId}, Status={filtro.Status}");

            var itens = await _service.ListAsync(filtro, ct);

            Console.WriteLine($"[Controller] retornou {itens.Count} itens");
            return Ok(itens);
        }
        [HttpPost]
        public async Task<ActionResult<LembreteDto>> Create(
        [FromBody] LembreteCreateDto dto,
        CancellationToken ct)
        {
            var criado = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LembreteDto>> Update(
        int id,
        [FromBody] LembreteCreateDto dto,
        CancellationToken ct)
        {
            var atualizado = await _service.UpdateAsync(id, dto, ct);
            if (atualizado is null) return NotFound();
            return Ok(atualizado);
        }
        [HttpPatch("{id:int}/concluir")]
        public async Task<IActionResult> Concluir(int id, CancellationToken ct)
        {
            await _service.ConcluirAsync(id, ct);
            return NoContent();
        }

        // PATCH api/lembretes/{id}/ignorar
        [HttpPatch("{id:int}/ignorar")]
        public async Task<IActionResult> Ignorar(int id, CancellationToken ct)
        {
            await _service.MarcarIgnoradoAsync(id, ct);
            return NoContent();
        }

        // DELETE api/lembretes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }

}