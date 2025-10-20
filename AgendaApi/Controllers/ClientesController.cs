using AgendaApi.Domain.DTOs;
using AgendaApi.Infra;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
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


        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <response code="201">Cliente criado com sucesso</response>
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
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
    }
}
