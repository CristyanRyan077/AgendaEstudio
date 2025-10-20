using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Interfaces;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _repository;

        public AgendamentoService(IAgendamentoRepository repository)
        {
            _repository = repository;
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

        public async Task<AgendamentoDto> CreateAsync(AgendamentoCreateDto dto)
        {
            var agendamento = dto.ToEntity();
            await _repository.AddAsync(agendamento);
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
    }
}
