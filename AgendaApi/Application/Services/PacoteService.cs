using AgendaApi.Domain.DTOs;
using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;

namespace AgendaApi.Application.Services
{
    public class PacoteService : IPacoteService
    {
        private readonly IPacoteRepository _repository;
        public PacoteService(IPacoteRepository repository) => _repository = repository;

        public async Task<Pacote> GetPacoteOrThrowAsync(int id)
        {
            return await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Pacote com Id {id} não encontrado.");
        }

        public async Task<IEnumerable<PacoteDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(p => p.ToDto());
        }

        public async Task<PacoteDto?> GetByIdAsync(int id)
        {
            var p = await GetPacoteOrThrowAsync(id);
            return p.ToDto();
        }

        public async Task<PacoteDto> CreateAsync(PacoteCreateDto dto)
        {
            var entity = dto.ToEntity();
            await _repository.AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<PacoteDto?> UpdateAsync(int id, PacoteUpdateDto dto)
        {
            var p = await GetPacoteOrThrowAsync(id);
            p.UpdateEntity(dto);
            await _repository.UpdateAsync(p);
            return p.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await GetPacoteOrThrowAsync(id);
            await _repository.DeleteAsync(p.Id);
            return true;
        }
    }
}