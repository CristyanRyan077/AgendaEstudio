using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Extensions.MiddleWares;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Interfaces;
using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Application.Services
{


    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _repository;
        private readonly IAgendamentoService _agendamentoService;
        public PagamentoService(IPagamentoRepository repository, IAgendamentoService agendamentoService)
        {
            _repository = repository;
            _agendamentoService = agendamentoService;
        }

        public async Task<Pagamento> GetPagamentoOrThrowAsync(int id)
        {
            return await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Pagamento com Id {id} não encontrado.");
        }
        public async Task<PagamentoDto> AdicionarPagamentoAsync(int agendamentoId, PagamentoCreateDto dto)
        {
            var agendamento = await _agendamentoService.GetAgendamentoOrThrowAsync(agendamentoId);

            var pagamento = new Pagamento
            {
                AgendamentoId = agendamentoId,
                Tipo = dto.Tipo,
                Valor = dto.Valor,
                DataPagamento = dto.DataPagamento,
                Metodo = dto.Metodo,
                Observacao = dto.Observacao,
                CreatedAt = DateTime.UtcNow   // sempre no servidor
            };
            await _repository.AddAsync(pagamento);
            return pagamento.ToDto();
        }

        public Task AtualizarPagamentoAsync(PagamentoUpdateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<HistoricoFinanceiroDto>> ListarHistoricoAsync(int agendamentoId)
        {
            var historico = _repository.ListarHistoricoAsync(agendamentoId);
            return historico;
        }

        public Task<ResumoAgendamentoDto> ObterResumoAgendamentoAsync(int agendamentoId)
        {
            var resumo = _repository.ObterResumoAgendamentoAsync(agendamentoId);
            return resumo;
        }
        public async Task<PagamentoDto?> GetByIdAsync(int id)
        {
            var pagamento = await GetPagamentoOrThrowAsync(id);
            return pagamento.ToDto();
        }

        public Task RemoverPagamentoAsync(int pagamentoID)
        {
            throw new NotImplementedException();
        }

        public Task RemoverProdutoDoAgendamentoAsync(int agendamentoProdutoId)
        {
            throw new NotImplementedException();
        }
    }
}
