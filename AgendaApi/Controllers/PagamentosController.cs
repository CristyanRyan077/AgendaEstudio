using AgendaApi.Infra.Interfaces;
using AgendaShared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoService _service;
        public PagamentosController(IPagamentoService service)
        {
            _service = service;
        }
        // GET: api/pagamentos/historico/{agendamentoId}
        [HttpGet("{id:int}/historico")]
        public async Task<ActionResult<IEnumerable<HistoricoFinanceiroDto>>> GetHistorico(int id)
        {
            var historico = await _service.ListarHistoricoAsync(id);
            return Ok(historico);
        }
        // GET : api/pagamentos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PagamentoDto>> GetById(int id)
        {
            var pagamento = await _service.GetByIdAsync(id);
            return Ok(pagamento); // se não existir, o middleware vai lançar NotFoundException
        }

        // POST: /api/agendamentos/{agendamentoId}/pagamentos
        [HttpPost("/api/agendamentos/{agendamentoId:int}/pagamentos")]
        public async Task<ActionResult<PagamentoDto>> Create([FromRoute] int agendamentoId, [FromBody] PagamentoCreateDto dto)
        {
            // (Opcional) validação rápida de campos do body -> ModelState
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var pagamento = await _service.AdicionarPagamentoAsync(agendamentoId, dto);
            // ideal: nomeie a rota do GetById para usar o CreatedAtAction corretamente
            return CreatedAtAction(nameof(GetById), new { id = pagamento.Id }, pagamento);
        }
        // PUT: api/pagamentos/5

        // DELETE: api/pagamentos/5

    }
}
