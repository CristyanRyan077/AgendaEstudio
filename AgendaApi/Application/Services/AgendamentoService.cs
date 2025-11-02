using AgendaApi.Domain;
using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Extensions.MiddleWares;
using AgendaApi.Infra;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Interfaces;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;

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
        public async Task<Result> ReagendarAsync(int agendamentoId, DateTime novaData, TimeSpan novoHorario)
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
    }
}
