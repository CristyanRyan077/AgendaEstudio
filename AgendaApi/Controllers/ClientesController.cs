using AgendaApi.Domain.Models;
using AgendaApi.Infra;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService servoce) => _service = servoce;


        // GET: api/v1/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
        {
            var clientes = await _service.GetAllAsync();
            return Ok(clientes);
        }
        // GET: api/v1/clientes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            var cliente = await _service.GetByIdAsync(id);
            return Ok(cliente); // se não existir, o middleware vai lançar NotFoundException
        }
        [HttpGet("{id:int}/Agendamentos")]
        public async Task<ActionResult<List<AgendamentoDto>>> GetAgendamentos(int id)
        {
            var agendamentos = await _service.GetAgendamentosAsync(id);
            return Ok(agendamentos);
        }

        [HttpGet("resumos")]
        public async Task<ActionResult<List<ClienteResumoDto>>> GetResumos()
        {
            var resumos = await _service.GetAllResumoAsync();
            return Ok(resumos);
        }
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Create(ClienteCreateDto dto)
        {
            var cliente = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        // PUT: api/v1/cliente/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClienteDto>> Update(int id, ClienteUpdateDto dto)
        {
            var atualizado = await _service.UpdateAsync(id, dto);
            return Ok(atualizado);
        }
        // DELETE: api/v1/cliente/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("paginado")]
        public async Task<ActionResult<PagedResult<ClienteResumoDto>>> GetClientes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? nome = null)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Parâmetros de paginação inválidos.");

            var result = await _service.ObterPaginadoAsync(page, pageSize,nome);
            return Ok(result);
        }
    }
}
