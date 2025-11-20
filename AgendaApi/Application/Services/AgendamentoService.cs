using AgendaApi.Domain;
using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Extensions.MiddleWares;
using AgendaApi.Infra;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Interfaces;
using AgendaApi.Models;
using AgendaShared;
using AgendaShared.DTOs;
using AgendaShared.Helpers;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AgendaApi.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _repository;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly AgendaContext _context;

        public AgendamentoService(IAgendamentoRepository repository, IPagamentoRepository pagamentoRepository, AgendaContext context)
        {
            _repository = repository;
            _pagamentoRepository = pagamentoRepository;
            _context = context;
        }
        public async Task<Agendamento> GetAgendamentoOrThrowAsync(int id)
        {
            return await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Agendamento com Id {id} não encontrado.");
        }

        public async Task<IEnumerable<AgendamentoDto>> GetAllAsync()
        {
            var agendamentos = await _repository.GetAllAsync();
            return agendamentos.Select(a => a.ToDto());
        }

        public async Task<AgendamentoDto?> GetByIdAsync(int id)
        {
            var agendamento = await GetAgendamentoOrThrowAsync(id);
            return agendamento.ToDto();
        }
        public async Task<Result> ReagendarAsync(int agendamentoId, DateTime novaData, TimeSpan? novoHorario)
        {
            var agendamento = await _context.Agendamentos.FindAsync(agendamentoId);
            if (agendamento == null) return Result.Fail("Agendamento não encontrado");

            var conflito = await _context.Agendamentos.AnyAsync(a => a.Data == novaData && a.Horario == novoHorario);
            if (conflito) return Result.Fail("Este horario está ocupado");

            agendamento.Data = novaData;
            agendamento.Horario = novoHorario;

            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> UpdateStatus(int id, StatusAgendamento novoStatus)
        {
           
            var agendamento = await _context.Agendamentos
                .Include(a => a.Pagamentos) 
                .FirstOrDefaultAsync(a => a.Id == id);
            if (agendamento == null) return Result.Fail($"Agendamento com ID {id} não encontrado.");

            var pago = agendamento.Pagamentos.Sum(p => p.Valor);

            if (agendamento.Valor <= pago)
            {
                agendamento.Status = novoStatus;
                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            return Result.Ok();

        }
        public async Task<Result> AtualizarStatusAgendamentosVencidosAsync()
        {
            // Pega a data de "hoje"
            var hoje = DateTime.Today;

            try
            {
                // 1. A LÓGICA DA BUSCA:
                //    - Status == Confirmado
                //    - Data < Hoje (Agendamento está no passado)
                //    - Soma dos Pagamentos < Valor Total
                var linhasAfetadas = await _context.Agendamentos
                    .Where(a => a.Status == StatusAgendamento.Confirmado &&
                                a.Data < hoje &&
                                a.Pagamentos.Sum(p => p.Valor) < a.Valor)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(a => a.Status, StatusAgendamento.Pendente)); // 2. A ATUALIZAÇÃO

                // Log para saber o que aconteceu
                System.Diagnostics.Debug.WriteLine($"[Job Vencidos] {linhasAfetadas} agendamentos atualizados para Pendente.");

                return Result.Ok();
            }
            catch (Exception ex)
            {
                // Logar o erro
                System.Diagnostics.Debug.WriteLine($"[Job Vencidos] Erro: {ex.Message}");
                return Result.Fail("Erro ao atualizar status dos agendamentos vencidos.");
            }
        }
        public async Task<AgendamentoDto> CreateAsync(AgendamentoCreateDto dto)
        {
            var agendamento = dto.ToEntity();

            await _repository.AddAsync(agendamento);

            if (dto.PagamentoInicial is not null)
            {
                var p = new Pagamento
                {
                    AgendamentoId = agendamento.Id,
                    Valor = dto.PagamentoInicial.Valor,
                    DataPagamento = dto.PagamentoInicial.DataPagamento == default ? DateTime.UtcNow : dto.PagamentoInicial.DataPagamento,
                    Metodo = dto.PagamentoInicial.Metodo,
                    Observacao = dto.PagamentoInicial.Observacao
                };
                await _pagamentoRepository.AddAsync(p);
            }
            return agendamento.ToDto();
        }

        public async Task<AgendamentoDto?> UpdateAsync(int id, AgendamentoUpdateDto dto)
        {
            var agendamento = await GetAgendamentoOrThrowAsync(id);
            agendamento.UpdateEntity(dto);
            await _repository.UpdateAsync(agendamento);

            return agendamento.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {

            var agendamento = await GetAgendamentoOrThrowAsync(id);
            await _repository.DeleteAsync(agendamento.Id);
            return true;
        }
        public async Task<IEnumerable<AgendamentoDto>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            var agendamentos = await _repository.GetPorPeriodoAsync(inicio, fim);
            return agendamentos.Select(a => a.ToDto());
        }
        public async Task<List<AgendamentoDto>> SearchAgendamentosAsync(AgendamentoSearchFilter filter)
        {
            if (filter == null ||
             (string.IsNullOrWhiteSpace(filter.SearchTerm) &&
             (filter.TiposDeFotoSelecionados == null || !filter.TiposDeFotoSelecionados.Any()) &&
             !filter.ClienteId.HasValue))
            {
                return new List<AgendamentoDto>();
            }
            var query = _context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Crianca)
            .Include(a => a.Servico)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var normalizedTerm = filter.SearchTerm.Trim().ToLowerInvariant();

                if (normalizedTerm.StartsWith("#") && int.TryParse(normalizedTerm.Substring(1), out int clienteId))
                {
                    var agendamentosById = await query
                        .Where(a => a.ClienteId == clienteId)
                        .ToListAsync();

                    return agendamentosById.Select(a => a.ToDto()).ToList();
                }

                query = query.Where(a =>
                    a.Cliente.Nome.ToLower().Contains(normalizedTerm) ||
                    a.Cliente.Telefone.EndsWith(normalizedTerm));
            }

            if (filter.TiposDeFotoSelecionados != null && filter.TiposDeFotoSelecionados.Any())
            {
                // Esta é a chave: O método .Contains() em uma lista de enums é EF-Core friendly.
                // Ele se traduz para o operador SQL "IN" (a.Servico.TipoFoto IN (Album, Painel)).
                var selectedEnums = filter.TiposDeFotoSelecionados;
                query = query.Where(a => selectedEnums.Contains(a.Entrega));
            }

            // 3. Mapeia para DTOs e retorna
            var agendamentosEncontrados = await query
            .OrderByDescending(a => a.Data) // Agendamentos mais recentes primeiro
            .Take(100) // Limita para evitar sobrecarga
            .ToListAsync();
            return agendamentosEncontrados.Select(a => a.ToDto()).ToList();
        }
       

    }
}
