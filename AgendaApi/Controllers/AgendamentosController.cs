using AgendaApi.Interfaces;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamentoService _service;

        public AgendamentosController(IAgendamentoService service)
        {
            _service = service;
        }

        // GET: api/v1/agendamento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoDto>>> GetAll()
        {
            var agendamentos = await _service.GetAllAsync();
            return Ok(agendamentos);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AgendamentoDto>> GetById(int id)
        {
            var agendamento = await _service.GetByIdAsync(id);
            return Ok(agendamento); // se não existir, o middleware vai lançar NotFoundException
        }
        // POST: api/v1/agendamento
        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>> Create(AgendamentoCreateDto dto)
        {
            var novoAgendamento = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = novoAgendamento.Id }, novoAgendamento);
        }
        // PUT: api/v1/agendamento/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AgendamentoDto>> Update(int id, AgendamentoUpdateDto dto)
        {
            var atualizado = await _service.UpdateAsync(id, dto);
            return Ok(atualizado);
        }
        // DELETE: api/v1/agendamento/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }


    }
}
